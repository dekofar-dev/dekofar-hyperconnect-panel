using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ResponseTemplates.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Handlers
{
    public class UpdateResponseTemplateHandler : IRequestHandler<UpdateResponseTemplateCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpdateResponseTemplateHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(UpdateResponseTemplateCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var template = await _context.ResponseTemplates
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (template == null)
                throw new InvalidOperationException("Template not found");

            // Only owner can modify non-global templates
            if (!template.IsGlobal && template.CreatedByUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException();

            template.Title = request.Title;
            template.Body = request.Body;
            template.IsGlobal = request.IsGlobal;
            template.ModuleScope = request.ModuleScope;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
