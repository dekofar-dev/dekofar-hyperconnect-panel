using System;
using System.Threading;
using System.Threading.Tasks;
using Dekofar.HyperConnect.Application.Common.Interfaces;
using Dekofar.HyperConnect.Application.UIPreferences.DTOs;
using Dekofar.HyperConnect.Application.UIPreferences.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dekofar.HyperConnect.Application.UIPreferences.Handlers
{
    public class GetUserUiPreferenceHandler : IRequestHandler<GetUserUiPreferenceQuery, UserUiPreferenceDto?>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public GetUserUiPreferenceHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<UserUiPreferenceDto?> Handle(GetUserUiPreferenceQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.UserId == null)
                throw new UnauthorizedAccessException();

            var pref = await _context.UserUIPreferences
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == _currentUser.UserId && p.ModuleKey == request.ModuleKey, cancellationToken);

            if (pref == null) return null;

            return new UserUiPreferenceDto
            {
                ModuleKey = pref.ModuleKey,
                PreferenceJson = pref.PreferenceJson
            };
        }
    }
}
