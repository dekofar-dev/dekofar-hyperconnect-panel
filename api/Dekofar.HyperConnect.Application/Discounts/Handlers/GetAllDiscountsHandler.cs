using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Discounts.DTOs;
using Dekofar.HyperConnect.Application.Discounts.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Discounts.Handlers
{
    public class GetAllDiscountsHandler : IRequestHandler<GetAllDiscountsQuery, List<DiscountDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllDiscountsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DiscountDto>> Handle(GetAllDiscountsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Discounts.AsQueryable();

            if (request.OnlyActive)
            {
                var now = DateTime.UtcNow;
                query = query.Where(d => d.IsActive && (!d.ValidFrom.HasValue || d.ValidFrom <= now) && (!d.ValidTo.HasValue || d.ValidTo >= now));
            }

            return await query.Select(d => new DiscountDto
            {
                Id = d.Id,
                Name = d.Name,
                Type = d.Type.ToString(),
                Value = d.Value,
                IsActive = d.IsActive,
                CreatedByUserId = d.CreatedByUserId,
                CreatedAt = d.CreatedAt,
                ValidFrom = d.ValidFrom,
                ValidTo = d.ValidTo
            }).ToListAsync(cancellationToken);
        }
    }
}
