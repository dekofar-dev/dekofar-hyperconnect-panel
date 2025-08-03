using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Discounts.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Discounts.Handlers
{
    public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public CreateDiscountHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Guid> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var discount = new Discount
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Type = request.Type,
                Value = request.Value,
                IsActive = request.IsActive,
                CreatedByUserId = _currentUser.UserId.Value,
                CreatedAt = DateTime.UtcNow,
                ValidFrom = request.ValidFrom,
                ValidTo = request.ValidTo
            };

            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync(cancellationToken);

            return discount.Id;
        }
    }
}
