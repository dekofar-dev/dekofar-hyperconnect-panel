using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Notes.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Notes.Handlers
{
    public class AddNoteHandler : IRequestHandler<AddNoteCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AddNoteHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(AddNoteCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var note = new Note
            {
                Id = Guid.NewGuid(),
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Text = request.Text,
                CreatedByUserId = _currentUser.UserId.Value,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Notes.AddAsync(note, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return note.Id;
        }
    }
}
