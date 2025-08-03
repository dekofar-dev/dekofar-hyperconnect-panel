using Dekofar.HyperConnect.Application.AllowedAdminIps.DTOs;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    // İzin verilen yönetici IP adresleri için servis implementasyonu
    public class AllowedAdminIpService : IAllowedAdminIpService
    {
        // Veri erişimi için genel depo
        private readonly IRepository<AllowedAdminIp> _repository;

        // Servis için kurucu
        public AllowedAdminIpService(IRepository<AllowedAdminIp> repository)
        {
            _repository = repository;
        }

        // Tüm izinli IP adreslerini döner
        public async Task<List<AllowedAdminIpDto>> GetAllAsync()
        {
            return await _repository.GetAll()
                .Select(x => new AllowedAdminIpDto
                {
                    Id = x.Id,
                    IpAddress = x.IpAddress,
                    Description = x.Description,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync();
        }

        // Belirli bir IP kaydını getirir
        public async Task<AllowedAdminIpDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            return new AllowedAdminIpDto
            {
                Id = entity.Id,
                IpAddress = entity.IpAddress,
                Description = entity.Description,
                CreatedAt = entity.CreatedAt
            };
        }

        // Yeni bir IP kaydı oluşturur
        public async Task<AllowedAdminIpDto> CreateAsync(AllowedAdminIpDto dto)
        {
            var entity = new AllowedAdminIp
            {
                IpAddress = dto.IpAddress,
                Description = dto.Description
            };
            var created = await _repository.AddAsync(entity);
            dto.Id = created.Id;
            dto.CreatedAt = created.CreatedAt;
            return dto;
        }

        // IP kaydını günceller
        public async Task<AllowedAdminIpDto?> UpdateAsync(int id, AllowedAdminIpDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;
            entity.IpAddress = dto.IpAddress;
            entity.Description = dto.Description;
            var updated = await _repository.UpdateAsync(entity);
            return new AllowedAdminIpDto
            {
                Id = updated.Id,
                IpAddress = updated.IpAddress,
                Description = updated.Description,
                CreatedAt = updated.CreatedAt
            };
        }

        // IP kaydını siler
        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
