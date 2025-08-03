using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.ResponseTemplates.DTOs;
using Dekofar.HyperConnect.Application.ResponseTemplates.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Handlers
{
    public class GetResponseTemplatesHandler : IRequestHandler<GetResponseTemplatesQuery, List<ResponseTemplateDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetResponseTemplatesHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<List<ResponseTemplateDto>> Handle(GetResponseTemplatesQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var query = _context.ResponseTemplates
                .Where(t => t.IsGlobal || t.CreatedByUserId == _currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(request.Module))
                query = query.Where(t => t.ModuleScope == request.Module);

            return await query
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
                .ToListAsync(cancellationToken);
        }
    }
}
