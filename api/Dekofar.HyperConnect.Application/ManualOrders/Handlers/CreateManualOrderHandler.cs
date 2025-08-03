using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ManualOrders.Commands;
using Dekofar.HyperConnect.Application.OrderCommissions.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Dekofar.HyperConnect.Application.ManualOrders.Handlers
{
    public class CreateManualOrderHandler : IRequestHandler<CreateManualOrderCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;

        public CreateManualOrderHandler(IApplicationDbContext context, ICurrentUserService currentUser, IMediator mediator)
        {
            _context = context;
            _currentUser = currentUser;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateManualOrderCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var order = new ManualOrder
            {
                CustomerName = request.CustomerName,
                CustomerSurname = request.CustomerSurname,
                Phone = request.Phone,
                Email = request.Email,
                Address = request.Address,
                City = request.City,
                District = request.District,
                PaymentType = request.PaymentType,
                OrderNote = request.OrderNote,
                CreatedByUserId = _currentUser.UserId.Value,
                CreatedAt = DateTime.UtcNow,
                Status = ManualOrderStatus.Pending
            };

            foreach (var item in request.Items)
            {
                order.Items.Add(new ManualOrderItem
                {
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Total = item.Price * item.Quantity
                });
            }

            var subTotal = order.Items.Sum(i => i.Total);

            decimal discountAmount = 0m;
            if (request.DiscountId.HasValue || !string.IsNullOrEmpty(request.DiscountName))
            {
                Discount? discount = null;
                if (request.DiscountId.HasValue)
                {
                    discount = await _context.Discounts.FindAsync(new object?[] { request.DiscountId.Value }, cancellationToken);
                }
                else if (!string.IsNullOrEmpty(request.DiscountName))
                {
                    discount = await _context.Discounts.FirstOrDefaultAsync(d => d.Name == request.DiscountName, cancellationToken);
                }

                if (discount != null && discount.IsActive &&
                    (!discount.ValidFrom.HasValue || discount.ValidFrom <= DateTime.UtcNow) &&
                    (!discount.ValidTo.HasValue || discount.ValidTo >= DateTime.UtcNow))
                {
                    order.DiscountName = discount.Name;
                    order.DiscountType = discount.Type.ToString();
                    order.DiscountValue = discount.Value;

                    discountAmount = discount.Type == DiscountType.Percentage
                        ? subTotal * discount.Value / 100m
                        : discount.Value;
                }
            }

            order.TotalAmount = subTotal - discountAmount;
            order.BonusAmount = order.TotalAmount * 0.1m; // simple commission calculation

            await _context.ManualOrders.AddAsync(order, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _mediator.Send(new CreateOrderCommissionCommand(order.Id, order.CreatedByUserId, order.TotalAmount), cancellationToken);

            return order.Id;
        }
    }
}

