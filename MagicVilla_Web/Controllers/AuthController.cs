using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using MagicVillaWeb.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MagicVilla_Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _contextAuth;
        public AuthController(IAuthService contextAuth)
        {
            _contextAuth = contextAuth;
        }
		[HttpGet]
        public IActionResult Login()
		{
			LoginRequestDTO loginRequestDTO = new();
			return View(loginRequestDTO);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDTO loginRequestDTO)
		{
			APIResponse response = await _contextAuth.LoginAsync<APIResponse>(loginRequestDTO);
			if (response !=null && response.IsSuccess)
			{
				LoginResponseDTO model=JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(response.Result));

				var handler = new JwtSecurityTokenHandler();
				var jwt=handler.ReadJwtToken(model.Token);


				var identity=new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

				identity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
				identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(n => n.Type == "role").Value));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken, model.Token);
				return RedirectToAction("Index","Home");
			}
			else
			{
				ModelState.AddModelError("CustomError", response.ErrorMessages.FirstOrDefault());
				return View(loginRequestDTO);
			}
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrationRequestDTO registrationRequestDTO)
		{
			APIResponse result=await _contextAuth.RegisterAsync<APIResponse>(registrationRequestDTO);
			if (result !=null &&result.IsSuccess)
				return RedirectToAction("Login");
			return View();
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Index", "Home");
        }
		public async Task<IActionResult> AccessDenied()
		{
			return View();
		}
	}
}
