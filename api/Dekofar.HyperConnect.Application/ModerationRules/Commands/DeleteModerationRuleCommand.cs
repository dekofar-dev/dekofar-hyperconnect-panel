using MediatR;

namespace Dekofar.HyperConnect.Application.ModerationRules.Commands
{
    public record DeleteModerationRuleCommand(int Id) : IRequest;
}
