using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.OrderCommissions.Commands
{
    public record CreateOrderCommissionCommand(Guid OrderId, Guid UserId, decimal TotalAmount) : IRequest<Guid>;
}
