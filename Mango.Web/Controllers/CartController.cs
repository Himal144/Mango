using Mango.Web.Models;
using Mango.Web.Models.CartAPIDto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        public async Task<IActionResult> CartIndex()
        {
            CartDto cartDto = await LoadCartBasedOnLoggedInUser();
            return View(cartDto);
        }

        private async Task<CartDto> LoadCartBasedOnLoggedInUser() {
            try
            {
                var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value;
                ResponseDto response = await _cartService.GetCartByUserIdAsync(userId);
                if (response != null && response.IsSuccess) { 
                CartDto cartDto = JsonConvert.DeserializeObject<CartDto>(Convert.ToString(response.Result));
                    return cartDto;
                }
                return new CartDto();
            }
            catch (Exception ex) {
                return new CartDto();
            }
        }
    }
}
