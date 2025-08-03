using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Auth
{
    // Kullanıcı giriş isteğini temsil eden model
    public class LoginRequest
    {
        // Kullanıcının e-posta adresi
        public string Email { get; set; } = null!;
        // Kullanıcının girdiği şifre
        public string Password { get; set; } = null!;
    }
}
