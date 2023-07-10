using MagicVilla_Web.Models.DTO;

namespace MagicVillaWeb.Models.DTO
{
	public class LoginResponseDTO
	{
        public UserDTO? User { get; set; }
        public string? Token { get; set; }
    }
}
