using System;
using MediatR;

namespace Dekofar.HyperConnect.Application.UserMessages.Commands
{
    public class MarkMessagesAsReadCommand : IRequest<int>
    {
        public Guid ChatUserId { get; }

        public MarkMessagesAsReadCommand(Guid chatUserId)
        {
            ChatUserId = chatUserId;
        }
    }
}
