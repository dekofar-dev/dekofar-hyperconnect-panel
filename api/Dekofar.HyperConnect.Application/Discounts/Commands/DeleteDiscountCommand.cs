using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.Discounts.Commands
{
    public record DeleteDiscountCommand(Guid Id) : IRequest<Unit>;
}
