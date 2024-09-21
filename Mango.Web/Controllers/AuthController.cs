using Mango.Web.Models.AuthAPIDto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Mango.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ActionName("Login")]
        public async Task<IActionResult> LoginPost(LoginRequestDto loginRequestDto)
        {
            var LoginResponse = await _authService.LoginAsync(loginRequestDto);
            if (LoginResponse.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>
                    (Convert.ToString(LoginResponse.Result));
                TempData["Success"] = LoginResponse.Message ;
                await SignInUser(loginResponseDto);
                return RedirectToAction("Index","Home");
            }
            TempData["error"] = LoginResponse.Message;
            return View(loginRequestDto);
        }

        public IActionResult Register()
        {
            var RoleList = new List<SelectListItem>() { 
             new SelectListItem{Text =SD.RoleCustomer,Value=SD.RoleCustomer },
             new SelectListItem{Text =SD.RoleAdmin,Value=SD.RoleAdmin },
            };
            ViewBag.RoleList = RoleList;
            return View();
        }

        [HttpPost]
        [ActionName("Register")]
        public async Task<IActionResult> RegisterPost(RegistrationRequestDto registrationRequestDto)
        {
            var RoleList = new List<SelectListItem>() {
             new SelectListItem{Text =SD.RoleCustomer,Value=SD.RoleCustomer },
             new SelectListItem{Text =SD.RoleAdmin,Value=SD.RoleAdmin },
            };
            var registrationResponse = await _authService.RegisterAsync(registrationRequestDto);


            if (registrationResponse.IsSuccess && registrationResponse.Result != null)
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleCustomer;
                }
                var assignRoleResponse = await _authService.AssignRoleAsync(registrationRequestDto);
                if (assignRoleResponse.IsSuccess) { 
                TempData["Success"] = "Registered successfully";
                return RedirectToAction(nameof(Login));
                }
                ViewBag.RoleList = RoleList;
                TempData["error"] = assignRoleResponse.Message;
                return View();
            }
            ViewBag.RoleList = RoleList;
            TempData["error"] = registrationResponse.Message;
            return View();
        }


        //Action for the logging in the user.

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(u=>u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
               jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,
              jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            var principal =new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}
