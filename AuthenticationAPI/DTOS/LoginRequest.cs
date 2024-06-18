using System.ComponentModel.DataAnnotations;

namespace AccessIdentityAPI.DTOS
{
    public class LoginRequest
    {
        [Required]
        public string LoginId { get; set; } = string.Empty;

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
