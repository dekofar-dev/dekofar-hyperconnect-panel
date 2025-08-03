using System;
using System.Collections.Generic;
using Dekofar.HyperConnect.Application.UserMessages.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.UserMessages.Queries
{
    public class GetChatMessagesQuery : IRequest<List<UserMessageDto>>
    {
        public Guid ChatUserId { get; }

        public GetChatMessagesQuery(Guid chatUserId)
        {
            ChatUserId = chatUserId;
        }
    }
}
