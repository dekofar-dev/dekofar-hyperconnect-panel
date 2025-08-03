using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.Notes.Commands
{
    public class AddNoteCommand : IRequest<Guid>
    {
        public string TargetType { get; set; } = default!;
        public Guid TargetId { get; set; }
        public string Text { get; set; } = default!;
    }
}
