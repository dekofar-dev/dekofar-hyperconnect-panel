using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Common.Models;
using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    /// <summary>
    /// Destek taleplerini listeleme sorgusunu ele alan handler sınıfı.
    /// Kullanıcının rolüne göre gerekli filtrelemeleri yapar ve sayfalı sonuç döner.
    /// </summary>
    public class GetAllSupportTicketsQueryHandler : IRequestHandler<GetAllSupportTicketsQuery, PagedResult<SupportTicketDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetAllSupportTicketsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Sorguyu işler ve kullanıcı yetkilerine göre filtrelenmiş destek taleplerini döner.
        /// </summary>
        public async Task<PagedResult<SupportTicketDto>> Handle(GetAllSupportTicketsQuery request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
                throw new UnauthorizedAccessException();

            // Temel sorgu tanımı
            var query = _context.SupportTickets.AsQueryable();

            // Yönetici değilse sadece kendi oluşturduğu veya kendisine atanan kayıtları görür
            if (!request.IsAdmin)
            {
                query = query.Where(t => t.CreatedByUserId == request.UserId || t.AssignedUserId == request.UserId);
            }

            // Kategori filtresi uygulanır
            if (request.CategoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == request.CategoryId);
            }

            // Durum filtresi uygulanır
            if (request.Status.HasValue)
            {
                query = query.Where(t => t.Status == request.Status);
            }

            // Başlık veya açıklamada arama yapılır
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var search = request.Search.ToLower();
                query = query.Where(t => t.Title.ToLower().Contains(search) || t.Description.ToLower().Contains(search));
            }

            // Toplam kayıt sayısı hesaplanır
            var totalCount = await query.CountAsync(cancellationToken);

            // Sayfalama uygulanarak veriler çekilir
            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(t => new SupportTicketDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedByUserId = t.CreatedByUserId,
                    AssignedUserId = t.AssignedUserId,
                    CategoryId = t.CategoryId,
                    OrderId = t.OrderId,
                    Status = t.Status,
                    Priority = t.Priority,
                    DueDate = t.DueDate,
                    FilePath = t.FilePath,
                    CreatedAt = t.CreatedAt,
                    LastUpdatedAt = t.LastUpdatedAt,
                    UnreadReplyCount = t.UnreadReplyCount
                })
                .ToListAsync(cancellationToken);

            // Sonuç modeli hazırlanır
            return new PagedResult<SupportTicketDto>
            {
                Items = tickets,
                TotalCount = totalCount,
                PageSize = request.PageSize,
                CurrentPage = request.PageNumber,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            };
        }
    }
}
