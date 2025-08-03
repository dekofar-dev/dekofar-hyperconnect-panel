using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.SupportCategories.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportCategories.Handlers
{
    public class DeleteSupportCategoryHandler : IRequestHandler<DeleteSupportCategoryCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteSupportCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteSupportCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.SupportCategories.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (category == null)
            {
                throw new Exception("Category not found");
            }

            _context.SupportCategories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
