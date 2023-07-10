using AutoMapper;
using MagicVilla.Data;
using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _context;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private string secretKey;
		private readonly IMapper _mapper;
        public UserRepository(ApplicationDbContext context,IConfiguration configuration
			, UserManager<ApplicationUser> userManager,IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
			_userManager = userManager;
			_mapper = mapper;
			_roleManager = roleManager;
        }
        public bool IsUniqueUser(string name)
		{
			var user=_context.ApplicationUsers.FirstOrDefault(x => x.Name == name);
			if (user == null)
				return true;
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = _context.ApplicationUsers
				.FirstOrDefault(n=>n.UserName.ToLower()==loginRequestDTO.UserName.ToLower());

			bool isValid=await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

			if (user == null||isValid==false)
			{
				return new LoginResponseDTO()
				{
					Token ="",
					User = null,
				};
			}

			var roles=await _userManager.GetRolesAsync(user);
			var tokenHandler=new JwtSecurityTokenHandler();
			var key=Encoding.ASCII.GetBytes(secretKey);
			var tokenDesceriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name,user.UserName.ToString()),
					new Claim(ClaimTypes.Role,roles.FirstOrDefault())
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token=tokenHandler.CreateToken(tokenDesceriptor);
			LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
			{
				Token = tokenHandler.WriteToken(token),
				User = _mapper.Map<UserDTO>(user),
			};
			return loginResponseDTO;
		}

		public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
		{
			ApplicationUser user = new()
			{
				Name = registrationRequestDTO.Name,
				UserName = registrationRequestDTO.UserName,
				Email=registrationRequestDTO.UserName,
				NormalizedEmail=registrationRequestDTO.UserName.ToUpper(),
			};
			try
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
				if (result.Succeeded) 
				{
					if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
					{
						await _roleManager.CreateAsync(new IdentityRole("admin"));
						await _roleManager.CreateAsync(new IdentityRole("Customer"));
					}
					await _userManager.AddToRoleAsync(user, "admin");
					var userToReturn=_context.ApplicationUsers
						.FirstOrDefault(n=>n.UserName== registrationRequestDTO.UserName);
					return _mapper.Map<UserDTO>(userToReturn);
				}

			}catch (Exception ex) 
			{

			}
			return new UserDTO();
		}
	}
}
