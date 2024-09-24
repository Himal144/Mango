using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDTO _responseDTO;
        

        public AuthAPIController(IAuthService authService )
        {
            _authService = authService;
            _responseDTO = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = errorMessage;
                return BadRequest(_responseDTO);

            }
            _responseDTO.Result = model;
            return Ok(_responseDTO);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null) {
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Username or password incorrect.";
                return BadRequest(_responseDTO);

            }
            _responseDTO.Result = loginResponse;
            

            _responseDTO.Message = "Logged in successfully.";
            return Ok(_responseDTO);
        }


        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            var assignRoleResponse = await _authService.AssignRole(model.Email,model.Role);
            if (assignRoleResponse == null)
            { 
                _responseDTO.IsSuccess = false;
                _responseDTO.Message = "Unable to create the role.";
                return BadRequest(_responseDTO);

            }
            _responseDTO.Result = assignRoleResponse;
            _responseDTO.Message = "Role created successfully successfully.";
            return Ok(_responseDTO);
        }
    }
}
