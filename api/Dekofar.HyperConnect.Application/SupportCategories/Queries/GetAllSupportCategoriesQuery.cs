using Dekofar.HyperConnect.Application.SupportCategories.DTOs;
using MediatR;
using System.Collections.Generic;

namespace Dekofar.HyperConnect.Application.SupportCategories.Queries
{
    public record GetAllSupportCategoriesQuery() : IRequest<List<SupportCategoryDto>>;
}
