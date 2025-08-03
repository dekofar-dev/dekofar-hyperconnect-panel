using System.Collections.Generic;
using Dekofar.HyperConnect.Application.ResponseTemplates.DTOs;
using MediatR;

namespace Dekofar.HyperConnect.Application.ResponseTemplates.Queries
{
    public class GetResponseTemplatesQuery : IRequest<List<ResponseTemplateDto>>
    {
        public string? Module { get; }
        public GetResponseTemplatesQuery(string? module)
        {
            Module = module;
        }
    }
}
