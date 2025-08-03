using System.ComponentModel.DataAnnotations;

namespace Dekofar.API.Controllers
{
    public class PinDto
    {
        [Required]
        [RegularExpression("^\\d{4}$", ErrorMessage = "PIN must be exactly 4 digits.")]
        public string Pin { get; set; } = string.Empty;
    }
}
