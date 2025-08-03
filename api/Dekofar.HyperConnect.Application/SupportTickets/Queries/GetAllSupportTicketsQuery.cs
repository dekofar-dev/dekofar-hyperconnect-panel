using Dekofar.HyperConnect.Application.Common.Models;
using Dekofar.HyperConnect.Application.SupportTickets.DTOs;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using System;

namespace Dekofar.HyperConnect.Application.SupportTickets.Queries
{
    /// <summary>
    /// Tüm destek taleplerini listelemek için kullanılan sorgu nesnesi.
    /// Rol ve filtre bilgilerinin tamamı bu sınıf üzerinden handler'a aktarılır.
    /// </summary>
    public class GetAllSupportTicketsQuery : IRequest<PagedResult<SupportTicketDto>>
    {
        /// <summary>
        /// Listeyi çeken kullanıcının benzersiz kimliği
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Kullanıcının yönetici olup olmadığı bilgisi
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// İstenen sayfa numarası (1 tabanlı)
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// Sayfa başına gösterilecek kayıt sayısı
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Filtrelenecek kategori kimliği (isteğe bağlı)
        /// </summary>
        public Guid? CategoryId { get; set; }

        /// <summary>
        /// Filtrelenecek durum bilgisi (isteğe bağlı)
        /// </summary>
        public SupportTicketStatus? Status { get; set; }

        /// <summary>
        /// Başlık veya açıklama üzerinde arama yapılacak metin
        /// </summary>
        public string? Search { get; set; }
    }
}
