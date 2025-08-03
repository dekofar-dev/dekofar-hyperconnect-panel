using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Notes.DTOs;
using Dekofar.HyperConnect.Application.Notes.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Notes.Handlers
{
    public class GetNotesByTargetHandler : IRequestHandler<GetNotesByTargetQuery, List<NoteDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetNotesByTargetHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<NoteDto>> Handle(GetNotesByTargetQuery request, CancellationToken cancellationToken)
        {
            return await _context.Notes
                .AsNoTracking()
                .Where(n => n.TargetType == request.TargetType && n.TargetId == request.TargetId)
                .OrderBy(n => n.CreatedAt)
                .Select(n => new NoteDto
                {
                    Id = n.Id,
                    TargetType = n.TargetType,
                    TargetId = n.TargetId,
                    Text = n.Text,
                    CreatedByUserId = n.CreatedByUserId,
                    CreatedAt = n.CreatedAt
                })
                .ToListAsync(cancellationToken);
        }
    }
}
