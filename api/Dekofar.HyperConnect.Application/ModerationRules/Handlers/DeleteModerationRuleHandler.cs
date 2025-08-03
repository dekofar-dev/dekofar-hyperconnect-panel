using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ModerationRules.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ModerationRules.Handlers
{
    public class DeleteModerationRuleHandler : IRequestHandler<DeleteModerationRuleCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteModerationRuleHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteModerationRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _context.ModerationRules
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (rule == null)
                return;

            _context.ModerationRules.Remove(rule);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
