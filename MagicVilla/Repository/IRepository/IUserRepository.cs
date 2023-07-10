using MagicVilla.Models;
using MagicVilla.Models.DTO;

namespace MagicVilla.Repository.IRepository
{
	public interface IUserRepository
	{
		bool IsUniqueUser(string name);
		Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
		Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO);
	}
}
