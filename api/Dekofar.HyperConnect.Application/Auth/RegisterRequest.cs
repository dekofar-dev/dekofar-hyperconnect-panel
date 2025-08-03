using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dekofar.HyperConnect.Application.Auth
{
    // Yeni kullanıcı kayıt isteğini temsil eden model
    public class RegisterRequest
    {
        // Kullanıcının tam adı
        public string FullName { get; set; } = null!;
        // Kullanıcının e-posta adresi
        public string Email { get; set; } = null!;
        // Kullanıcının belirlediği şifre
        public string Password { get; set; } = null!;
    }
}
