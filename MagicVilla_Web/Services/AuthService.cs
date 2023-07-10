using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Services.IServices;
using MagicVillaWeb.Models.DTO;
using Microsoft.Extensions.Configuration;

namespace MagicVilla_Web.Services
{
	public class AuthService :BaseService, IAuthService
	{
		private readonly IHttpClientFactory _httpClientFactory;
		private string villaUrl;
        public AuthService(IHttpClientFactory httpClientFactory,IConfiguration configuration):base(httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
			villaUrl= configuration.GetValue<string>("ServiceUrls:VillaAPI");
		}
        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = loginRequestDTO,
				Url = villaUrl + "/api/userAuth/login"
			});
		}

		public Task<T> RegisterAsync<T>(RegistrationRequestDTO registerRequestDTO)
		{
			return SendAsync<T>(new APIRequest()
			{
				ApiType = SD.ApiType.POST,
				Data = registerRequestDTO,
				Url = villaUrl + "/api/userAuth/register"
			});
		}
	}
}
