using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Infrastructure.Services
{
    /// <summary>
    /// Kullanıcı çalışma oturumlarını yöneten servis.
    /// </summary>
    public class WorkSessionService : IWorkSessionService
    {
        private readonly IApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkSessionService(IApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Kullanıcı için oturumu başlatır ve IsOnline alanını günceller.
        /// </summary>
        public async Task<WorkSession> StartSessionAsync(Guid userId, string? ip)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) throw new Exception("Kullanıcı bulunamadı.");

            var session = new WorkSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StartTime = DateTime.UtcNow,
                StartIp = ip
            };

            user.IsOnline = true;
            user.LastSeen = DateTime.UtcNow;

            _context.WorkSessions.Add(session);
            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            return session;
        }

        /// <summary>
        /// Aktif oturumu sonlandırır ve kullanıcıyı offline yapar.
        /// </summary>
        public async Task<WorkSession?> EndSessionAsync(Guid userId)
        {
            var session = await _context.WorkSessions
                .Where(x => x.UserId == userId && x.EndTime == null)
                .OrderByDescending(x => x.StartTime)
                .FirstOrDefaultAsync();

            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.IsOnline = false;
                user.LastSeen = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }

            if (session == null)
            {
                await _context.SaveChangesAsync();
                return null;
            }

            session.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return session;
        }

        /// <summary>
        /// Kullanıcının geçmiş oturumlarını listeler.
        /// </summary>
        public async Task<List<WorkSession>> GetUserSessionsAsync(Guid userId)
        {
            return await _context.WorkSessions
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Tüm oturumları içeren rapor döner (sadece admin).
        /// </summary>
        public async Task<List<WorkSession>> GetReportAsync()
        {
            return await _context.WorkSessions
                .Include(x => x.User)
                .OrderByDescending(x => x.StartTime)
                .ToListAsync();
        }
    }
}
