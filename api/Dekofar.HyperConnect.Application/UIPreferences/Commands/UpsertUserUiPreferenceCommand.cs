using MediatR;

namespace Dekofar.HyperConnect.Application.UIPreferences.Commands
{
    public class UpsertUserUiPreferenceCommand : IRequest<Unit>
    {
        public string ModuleKey { get; set; } = default!;
        public string PreferenceJson { get; set; } = default!;
    }
}
