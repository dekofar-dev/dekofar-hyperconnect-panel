using System;

namespace Dekofar.HyperConnect.Application.AllowedAdminIps.DTOs
{
    // Yönetici paneli için izin verilen IP adresi bilgisini taşıyan DTO
    public class AllowedAdminIpDto
    {
        // Kaydın benzersiz kimliği
        public int Id { get; set; }

        // İzin verilen IP adresi
        public string IpAddress { get; set; } = string.Empty;

        // IP adresine ilişkin açıklama
        public string? Description { get; set; }

        // Kaydın oluşturulma zamanı
        public DateTime CreatedAt { get; set; }
    }
}
