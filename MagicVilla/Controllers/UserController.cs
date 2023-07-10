using MagicVilla.Models;
using MagicVilla.Models.DTO;
using MagicVilla.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla.Controllers
{
	[Route("api/userAuth")]
	[ApiController]
	public class UserController : Controller
	{
		private readonly IUserRepository _contextUser;
		protected APIResponse _response;
		public UserController(IUserRepository contextUser)
		{
			_contextUser = contextUser;
			this._response = new();
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody]LoginRequestDTO loginRequestDTO)
		{
			var loginResponse=await _contextUser.Login(loginRequestDTO);
			if (loginResponse.User == null||string.IsNullOrEmpty(loginResponse.Token))
			{
				_response.StatusCode=HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("username or password is wrong");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess=true;
			_response.Result=loginResponse;
			return Ok(_response);
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
		{
			bool ifUserNameUnique=_contextUser.IsUniqueUser(registrationRequestDTO.UserName);
			if(!ifUserNameUnique) 
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("UserName already exist");
				return BadRequest(_response);
			}
			var user=await _contextUser.Register(registrationRequestDTO);
			if (user==null)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.IsSuccess = false;
				_response.ErrorMessages.Add("Error while registration");
				return BadRequest(_response);
			}
			_response.StatusCode = HttpStatusCode.OK;
			_response.IsSuccess = true;
			return Ok(_response);
		}
	}
}
