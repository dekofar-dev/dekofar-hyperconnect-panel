using Dekofar.HyperConnect.Application.ResponseTemplates.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Queries
{
    public record GetResponseTemplateByIdQuery(int Id) : IRequest<ResponseTemplateDto?>;
}
