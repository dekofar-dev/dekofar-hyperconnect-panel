using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.Services
{
    public class ModerationService : IModerationService
    {
        private readonly IApplicationDbContext _context;

        public ModerationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ModerationResult> CheckAsync(string? content, Guid? userId)
        {
            var result = new ModerationResult { Blocked = false };
            if (string.IsNullOrWhiteSpace(content))
                return result;

            var rules = await _context.ModerationRules
                .Where(r => r.IsActive)
                .ToListAsync();

            foreach (var rule in rules)
            {
                if (Regex.IsMatch(content, rule.Pattern, RegexOptions.IgnoreCase))
                {
                    result.Rule = rule;
                    if (rule.Severity == ModerationSeverity.Block)
                        result.Blocked = true;

                    await _context.ModerationLogs.AddAsync(new ModerationLog
                    {
                        Content = content,
                        RuleId = rule.Id,
                        Severity = rule.Severity,
                        UserId = userId,
                        CreatedAt = DateTime.UtcNow
                    });

                    await _context.SaveChangesAsync();
                    break;
                }
            }

            return result;
        }
    }
}
