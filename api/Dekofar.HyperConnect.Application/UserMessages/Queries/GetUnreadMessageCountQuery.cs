using MediatR;

namespace Dekofar.HyperConnect.Application.UserMessages.Queries
{
    public class GetUnreadMessageCountQuery : IRequest<int>
    {
    }
}
