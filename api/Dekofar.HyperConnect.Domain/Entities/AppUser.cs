using Microsoft.AspNetCore.Identity;
using System;

namespace Dekofar.HyperConnect.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }

        /// <summary>
        ///     Stored hashed representation of the user's 4-digit PIN.
        /// </summary>
        public string? HashedPin { get; set; }

        /// <summary>
        ///     Timestamp of the last PIN update for auditing purposes.
        /// </summary>
        public DateTime? PinLastUpdatedAt { get; set; }
    }
}