using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.SupportCategories.Commands
{
    public record DeleteSupportCategoryCommand(Guid Id) : IRequest<Unit>;
}
