using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Discounts.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Discounts.Handlers
{
    public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public UpdateDiscountHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
        {
            var discount = await _context.Discounts.FindAsync(new object?[] { request.Id }, cancellationToken);
            if (discount == null)
                throw new Exception("Discount not found");

            discount.Name = request.Name;
            discount.Type = request.Type;
            discount.Value = request.Value;
            discount.IsActive = request.IsActive;
            discount.ValidFrom = request.ValidFrom;
            discount.ValidTo = request.ValidTo;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
