using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.OrderCommissions.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.OrderCommissions.Queries
{
    public class GetCommissionsByUserQuery : IRequest<List<OrderCommissionDto>> { }

    public class GetCommissionsByUserQueryHandler : IRequestHandler<GetCommissionsByUserQuery, List<OrderCommissionDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetCommissionsByUserQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<OrderCommissionDto>> Handle(GetCommissionsByUserQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            return await _context.OrderCommissions
                .Where(c => c.UserId == _currentUser.UserId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new OrderCommissionDto
                {
                    Id = c.Id,
                    OrderId = c.OrderId,
                    UserId = c.UserId,
                    CommissionRate = c.CommissionRate,
                    EarnedAmount = c.EarnedAmount,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }

    public class GetUserTotalCommissionQuery : IRequest<decimal> { }

    public class GetUserTotalCommissionQueryHandler : IRequestHandler<GetUserTotalCommissionQuery, decimal>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetUserTotalCommissionQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<decimal> Handle(GetUserTotalCommissionQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            return await _context.OrderCommissions
                .Where(c => c.UserId == _currentUser.UserId)
                .SumAsync(c => c.EarnedAmount, cancellationToken);
        }
    }
}
