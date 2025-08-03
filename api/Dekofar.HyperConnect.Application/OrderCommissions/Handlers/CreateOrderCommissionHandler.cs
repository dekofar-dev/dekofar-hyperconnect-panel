using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.OrderCommissions.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.OrderCommissions.Handlers
{
    public class CreateOrderCommissionHandler : IRequestHandler<CreateOrderCommissionCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly INotificationService _notificationService;
        private const decimal CommissionRate = 0.05m;

        public CreateOrderCommissionHandler(IApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<Guid> Handle(CreateOrderCommissionCommand request, CancellationToken cancellationToken)
        {
            var commission = new OrderCommission
            {
                Id = Guid.NewGuid(),
                OrderId = request.OrderId,
                UserId = request.UserId,
                CommissionRate = CommissionRate,
                EarnedAmount = request.TotalAmount * CommissionRate,
                CreatedAt = DateTime.UtcNow
            };

            await _context.OrderCommissions.AddAsync(commission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _notificationService.SendToUserAsync(request.UserId, "CommissionCreated", new
            {
                orderId = request.OrderId,
                amount = commission.EarnedAmount,
                createdAt = commission.CreatedAt
            }, cancellationToken);

            return commission.Id;
        }
    }
}
