using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ModerationRules.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;

namespace Dekofar.HyperConnect.Application.ModerationRules.Handlers
{
    public class CreateModerationRuleHandler : IRequestHandler<CreateModerationRuleCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateModerationRuleHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateModerationRuleCommand request, CancellationToken cancellationToken)
        {
            var rule = new ModerationRule
            {
                Pattern = request.Pattern,
                Severity = request.Severity,
                Description = request.Description,
                IsActive = request.IsActive
            };

            await _context.ModerationRules.AddAsync(rule, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return rule.Id;
        }
    }
}
