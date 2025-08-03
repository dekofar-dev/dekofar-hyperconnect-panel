using Dekofar.HyperConnect.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Common.Interfaces
{
    /// <summary>
    /// Kullanıcı çalışma oturumlarını yöneten servis sözleşmesi.
    /// </summary>
    public interface IWorkSessionService
    {
        /// <summary>
        /// Kullanıcı için yeni bir oturum başlatır.
        /// </summary>
        Task<WorkSession> StartSessionAsync(Guid userId, string? ip);

        /// <summary>
        /// Kullanıcının aktif oturumunu sonlandırır.
        /// </summary>
        Task<WorkSession?> EndSessionAsync(Guid userId);

        /// <summary>
        /// Belirli kullanıcının oturum listesini döner.
        /// </summary>
        Task<List<WorkSession>> GetUserSessionsAsync(Guid userId);

        /// <summary>
        /// Tüm oturumlar için rapor üretir (admin).
        /// </summary>
        Task<List<WorkSession>> GetReportAsync();
    }
}
