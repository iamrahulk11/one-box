using Asp.Versioning;
using domain.DTOs;
using domain.DTOs.Requests.Login;
using domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers.v1.Login
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ControllerName("login")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _login;
        public LoginController(LoginService login)
        {
            _login = login;
        }

        [HttpGet("{username}")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ActionName("user-exists")]
        public IActionResult userExistsAsync([FromRoute] usernameRequestDto request_body)
        {
            (int, baseResponseDto<object>) _processResponse = _login.processUserExistsAsync(request_body);
            return StatusCode(_processResponse.Item1, _processResponse.Item2);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ActionName("create-user")]
        public async Task<IActionResult> createUserAsync([FromBody] userCreationRequestDto request_body)
        {
            (int, baseResponseDto<object>) _processResponse = await _login.processCreateUserAsync(request_body);
            return StatusCode(_processResponse.Item1, _processResponse.Item2);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ActionName("by-password")]
        public async Task<IActionResult> loginByPasswordAsync([FromBody] LoginRequestDto request_body)
        {
            (int, baseResponseDto<object>) _processResponse = await _login.processLoginByPasswordAsync(request_body);
            return StatusCode(_processResponse.Item1, _processResponse.Item2);
        }

        [HttpPost]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ActionName("refresh-token")]
        public async Task<IActionResult> refreshTokenAsync([FromBody] refreshTokenRequestDto request_body)
        {
            (int, baseResponseDto<object>) _processResponse = await _login.processRefreshTokenAsync(request_body);
            return StatusCode(_processResponse.Item1, _processResponse.Item2);
        }
    }
}