using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ResponseTemplates.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Handlers
{
    public class CreateResponseTemplateHandler : IRequestHandler<CreateResponseTemplateCommand, int>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateResponseTemplateHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<int> Handle(CreateResponseTemplateCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var template = new ResponseTemplate
            {
                Title = request.Title,
                Body = request.Body,
                IsGlobal = request.IsGlobal,
                CreatedByUserId = request.IsGlobal ? null : _currentUser.UserId,
                CreatedAt = DateTime.UtcNow,
                ModuleScope = request.ModuleScope
            };

            await _context.ResponseTemplates.AddAsync(template, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return template.Id;
        }
    }
}
