using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportCategories.DTOs;
using Dekofar.HyperConnect.Application.SupportCategories.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportCategories.Handlers
{
    public class GetAllSupportCategoriesHandler : IRequestHandler<GetAllSupportCategoriesQuery, List<SupportCategoryDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllSupportCategoriesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SupportCategoryDto>> Handle(GetAllSupportCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await _context.SupportCategories
                .Include(c => c.Roles)
                .Select(c => new SupportCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    Roles = c.Roles.Select(r => r.RoleName).ToList()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
