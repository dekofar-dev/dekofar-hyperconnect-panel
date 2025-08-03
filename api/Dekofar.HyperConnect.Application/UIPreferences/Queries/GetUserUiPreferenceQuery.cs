using Dekofar.HyperConnect.Application.UIPreferences.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.UIPreferences.Queries
{
    public record GetUserUiPreferenceQuery(string ModuleKey) : IRequest<UserUiPreferenceDto?>;
}
