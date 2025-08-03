using MediatR;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Commands
{
    public record DeleteResponseTemplateCommand(int Id) : IRequest;
}
