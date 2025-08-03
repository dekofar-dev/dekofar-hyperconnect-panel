using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ModerationRules.DTOs;
using Dekofar.HyperConnect.Application.ModerationRules.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ModerationRules.Handlers
{
    public class GetModerationRulesHandler : IRequestHandler<GetModerationRulesQuery, List<ModerationRuleDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetModerationRulesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ModerationRuleDto>> Handle(GetModerationRulesQuery request, CancellationToken cancellationToken)
        {
            return await _context.ModerationRules
                .Select(r => new ModerationRuleDto
                {
                    Id = r.Id,
                    Pattern = r.Pattern,
                    Severity = r.Severity,
                    Description = r.Description,
                    IsActive = r.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
