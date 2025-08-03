using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ResponseTemplates.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Handlers
{
    public class DeleteResponseTemplateHandler : IRequestHandler<DeleteResponseTemplateCommand>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public DeleteResponseTemplateHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task Handle(DeleteResponseTemplateCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var template = await _context.ResponseTemplates
                .FirstOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

            if (template == null)
                return;

            if (!template.IsGlobal && template.CreatedByUserId != _currentUser.UserId)
                throw new UnauthorizedAccessException();

            _context.ResponseTemplates.Remove(template);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
