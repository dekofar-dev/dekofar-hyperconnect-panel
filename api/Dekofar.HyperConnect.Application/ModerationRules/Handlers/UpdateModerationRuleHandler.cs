using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ModerationRules.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ModerationRules.Handlers
{
    public class UpdateModerationRuleHandler : IRequestHandler<UpdateModerationRuleCommand>
    {
        private readonly IApplicationDbContext _context;

        public UpdateModerationRuleHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateModerationRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = await _context.ModerationRules
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

            if (rule == null)
                throw new InvalidOperationException("Rule not found");

            rule.Pattern = request.Pattern;
            rule.Severity = request.Severity;
            rule.Description = request.Description;
            rule.IsActive = request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
