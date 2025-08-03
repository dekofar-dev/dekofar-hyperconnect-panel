using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UIPreferences.Commands;
using Dekofar.HyperConnect.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.UIPreferences.Handlers
{
    public class UpsertUserUiPreferenceHandler : IRequestHandler<UpsertUserUiPreferenceCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public UpsertUserUiPreferenceHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(UpsertUserUiPreferenceCommand request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var pref = await _context.UserUIPreferences
                .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId && p.ModuleKey == request.ModuleKey, cancellationToken);

            if (pref == null)
            {
                pref = new UserUIPreference
                {
                    Id = Guid.NewGuid(),
                    UserId = _currentUser.UserId.Value,
                    ModuleKey = request.ModuleKey,
                    PreferenceJson = request.PreferenceJson,
                    UpdatedAt = DateTime.UtcNow
                };
                await _context.UserUIPreferences.AddAsync(pref, cancellationToken);
            }
            else
            {
                pref.PreferenceJson = request.PreferenceJson;
                pref.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
