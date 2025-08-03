using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    // Yönetici paneline erişimine izin verilen IP adreslerini temsil eden varlık
    public class AllowedAdminIp
    {
        // IP kaydının benzersiz kimliği
        public int Id { get; set; }

        // İzin verilen IP adresi
        public string IpAddress { get; set; } = string.Empty;

        // IP adresi hakkında açıklama
        public string? Description { get; set; }

        // Kaydın oluşturulma tarihi
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
