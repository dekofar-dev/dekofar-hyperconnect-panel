using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ResponseTemplates.DTOs;
using Dekofar.HyperConnect.Application.ResponseTemplates.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Handlers
{
    public class GetResponseTemplateByIdHandler : IRequestHandler<GetResponseTemplateByIdQuery, ResponseTemplateDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetResponseTemplateByIdHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<ResponseTemplateDto?> Handle(GetResponseTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            return await _context.ResponseTemplates
                .Where(t => t.Id == request.Id && (t.IsGlobal || t.CreatedByUserId == _currentUser.UserId))
                .Select(t => new ResponseTemplateDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Body = t.Body,
                    IsGlobal = t.IsGlobal,
                    CreatedByUserId = t.CreatedByUserId,
                    CreatedAt = t.CreatedAt,
                    ModuleScope = t.ModuleScope
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
