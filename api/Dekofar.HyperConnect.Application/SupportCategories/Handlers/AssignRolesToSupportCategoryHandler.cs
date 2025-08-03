using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportCategories.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportCategories.Handlers
{
    public class AssignRolesToSupportCategoryHandler : IRequestHandler<AssignRolesToSupportCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public AssignRolesToSupportCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AssignRolesToSupportCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.SupportCategories
                .Include(c => c.Roles)
                .FirstOrDefaultAsync(c => c.Id == request.SupportCategoryId, cancellationToken);

            if (category == null)
            {
                throw new Exception("Category not found");
            }

            _context.SupportCategoryRoles.RemoveRange(category.Roles);

            foreach (var role in request.Roles.Distinct())
            {
                category.Roles.Add(new SupportCategoryRole
                {
                    Id = Guid.NewGuid(),
                    RoleName = role,
                    SupportCategoryId = category.Id
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
