using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Discounts.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Discounts.Handlers
{
    public class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public DeleteDiscountHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _context.Discounts.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (discount == null)
                throw new Exception("Discount not found");

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
