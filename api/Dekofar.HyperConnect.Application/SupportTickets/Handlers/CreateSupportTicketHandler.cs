using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportTickets.Commands;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Handlers
{
    public class CreateSupportTicketHandler : IRequestHandler<CreateSupportTicketCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IHostEnvironment _env;
        private readonly IModerationService _moderation;

        public CreateSupportTicketHandler(IApplicationDbContext context, ICurrentUserService currentUser, IHostEnvironment env, IModerationService moderation)
        {
            _context = context;
            _currentUser = currentUser;
            _env = env;
            _moderation = moderation;
        }

        public async Task<Guid> Handle(CreateSupportTicketCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var checkTitle = await _moderation.CheckAsync(request.Title, _currentUser.UserId);
            if (checkTitle.Blocked)
                throw new InvalidOperationException("Your message contains restricted content.");

            var checkDesc = await _moderation.CheckAsync(request.Description, _currentUser.UserId);
            if (checkDesc.Blocked)
                throw new InvalidOperationException("Your message contains restricted content.");

            var ticket = new SupportTicket
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                CreatedByUserId = _currentUser.UserId.Value,
                CategoryId = request.CategoryId,
                OrderId = request.OrderId,
                Priority = request.Priority,
                Status = SupportTicketStatus.Open,
                DueDate = request.DueDate,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            if (request.CategoryId.HasValue)
            {
                var roleNames = await _context.SupportCategoryRoles
                    .Where(r => r.SupportCategoryId == request.CategoryId.Value)
                    .Select(r => r.RoleName)
                    .ToListAsync(cancellationToken);

                if (roleNames.Any())
                {
                    var userIds = await (from u in _context.Users
                                         join ur in _context.UserRoles on u.Id equals ur.UserId
                                         join r in _context.Roles on ur.RoleId equals r.Id
                                         where roleNames.Contains(r.Name!)
                                         select u.Id)
                                         .ToListAsync(cancellationToken);

                    if (userIds.Any())
                    {
                        var counts = await _context.SupportTickets
                            .Where(t => t.Status != SupportTicketStatus.Closed && t.AssignedUserId != null && userIds.Contains(t.AssignedUserId.Value))
                            .GroupBy(t => t.AssignedUserId!.Value)
                            .Select(g => new { UserId = g.Key, Count = g.Count() })
                            .ToListAsync(cancellationToken);

                        var selected = userIds
                            .Select(id => new { Id = id, Count = counts.FirstOrDefault(c => c.UserId == id)?.Count ?? 0 })
                            .OrderBy(x => x.Count)
                            .ThenBy(x => x.Id)
                            .First();

                        ticket.AssignedUserId = selected.Id;
                    }
                }
            }

            if (request.File != null)
            {
                var uploads = Path.Combine(_env.ContentRootPath, "uploads");
                Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid() + Path.GetExtension(request.File.FileName);
                var fullPath = Path.Combine(uploads, fileName);
                using var stream = new FileStream(fullPath, FileMode.Create);
                await request.File.CopyToAsync(stream, cancellationToken);
                ticket.FilePath = fullPath;
            }

            _context.SupportTickets.Add(ticket);
            await _context.SaveChangesAsync(cancellationToken);

            return ticket.Id;
        }
    }
}
