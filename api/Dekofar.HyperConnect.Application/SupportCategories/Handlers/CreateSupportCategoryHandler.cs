using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportCategories.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportCategories.Handlers
{
    public class CreateSupportCategoryHandler : IRequestHandler<CreateSupportCategoryCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateSupportCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateSupportCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new SupportCategory
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            _context.SupportCategories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
