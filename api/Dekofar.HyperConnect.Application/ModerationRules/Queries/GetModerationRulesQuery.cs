using System.Collections.Generic;
using Dekofar.HyperConnect.Application.ModerationRules.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.ModerationRules.Queries
{
    public class GetModerationRulesQuery : IRequest<List<ModerationRuleDto>>
    {
    }
}
