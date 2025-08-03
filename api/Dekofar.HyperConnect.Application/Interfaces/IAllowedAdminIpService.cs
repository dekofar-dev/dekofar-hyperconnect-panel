using Dekofar.HyperConnect.Application.AllowedAdminIps.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Interfaces
{
    // İzin verilen yönetici IP adresleri için servis arayüzü
    public interface IAllowedAdminIpService
    {
        // Tüm izinli IP kayıtlarını döner
        Task<List<AllowedAdminIpDto>> GetAllAsync();

        // Belirli bir IP kaydını döner
        Task<AllowedAdminIpDto?> GetByIdAsync(int id);

        // Yeni bir IP kaydı oluşturur
        Task<AllowedAdminIpDto> CreateAsync(AllowedAdminIpDto dto);

        // Var olan IP kaydını günceller
        Task<AllowedAdminIpDto?> UpdateAsync(int id, AllowedAdminIpDto dto);

        // Belirli IP kaydını siler
        Task<bool> DeleteAsync(int id);
    }
}
