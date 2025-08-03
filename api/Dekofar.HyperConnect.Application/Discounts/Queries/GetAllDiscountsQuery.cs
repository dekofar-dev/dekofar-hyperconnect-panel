using Dekofar.HyperConnect.Application.Discounts.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Discounts.Queries
{
    public record GetAllDiscountsQuery(bool OnlyActive = false) : IRequest<List<DiscountDto>>;
}
