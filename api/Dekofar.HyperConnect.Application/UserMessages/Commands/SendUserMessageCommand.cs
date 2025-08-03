using System;
using Dekofar.HyperConnect.Application.UserMessages.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.UserMessages.Commands
{
    public class SendUserMessageCommand : IRequest<UserMessageDto>
    {
        public Guid ReceiverId { get; set; }
        public string? Text { get; set; }
        public string? FileUrl { get; set; }
        public string? FileType { get; set; }
        public long? FileSize { get; set; }
    }
}
