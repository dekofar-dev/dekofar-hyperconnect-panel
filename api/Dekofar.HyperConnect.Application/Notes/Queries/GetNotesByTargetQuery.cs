using Dekofar.HyperConnect.Application.Notes.DTOs;
using MediatR;
using System;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.Notes.Queries
{
    public class GetNotesByTargetQuery : IRequest<List<NoteDto>>
    {
        public string TargetType { get; }
        public Guid TargetId { get; }

        public GetNotesByTargetQuery(string targetType, Guid targetId)
        {
            TargetType = targetType;
            TargetId = targetId;
        }
    }
}
