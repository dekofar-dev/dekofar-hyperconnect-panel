using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    /// <summary>
    /// Kullanıcıların aktif çalışma sürelerini takip eden oturum kaydı.
    /// </summary>
    public class WorkSession
    {
        // Oturum benzersiz kimliği
        public Guid Id { get; set; }

        // Oturumu başlatan kullanıcının kimliği
        public Guid UserId { get; set; }

        // Kullanıcı navigasyon özelliği
        public ApplicationUser User { get; set; } = default!;

        // Oturum başlangıç zamanı
        public DateTime StartTime { get; set; }

        // Oturum bitiş zamanı (null ise devam ediyor)
        public DateTime? EndTime { get; set; }

        // Oturum başlangıcındaki IP adresi
        public string? StartIp { get; set; }

        // Oturum süresi (EndTime - StartTime)
        public TimeSpan? Duration => EndTime.HasValue ? EndTime - StartTime : null;
    }
}
