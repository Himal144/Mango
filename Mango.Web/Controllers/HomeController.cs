using Mango.Web.Models;
using Mango.Web.Models.CartAPIDto;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
		private readonly IProductService _productService;
		private readonly ICartService _cartService;


		public HomeController(IProductService productService,ICartService cartService)
		{
			_productService = productService;
			_cartService = cartService;

		}
		public async  Task<IActionResult> Index()
        {
			List<ProductDto> list = new();
			ResponseDto? response = await _productService.GetAllProductAsync();
			if (response != null && response.IsSuccess)
			{
				list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response.Message;
			}
			return View(list);
		}

		[Authorize]
		public async Task<IActionResult> ProductDetails(int productId)
		{
			ResponseDto? response = await _productService.GetProductByIdAsync(productId);
			ProductDto productDto = new ProductDto();
			if (response != null && response.IsSuccess)
			{ 
			 productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
		    }
            return View(productDto);
		}


        [Authorize]
		[HttpPost]
		[ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
			CartDto cartDto = new()
			{
				CartHeader = new CartHeaderDto()
				{
					UserId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault().Value
				}
			};

			CartDetailsDto cartDetails = new CartDetailsDto()
			{
				Count = productDto.Count,
				ProductId = productDto.ProductId
			};
			List<CartDetailsDto> cartDetailsDtos = new() { cartDetails};
			cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);
          
            if (response != null && response.IsSuccess)
            {
				TempData["Success"] = "Item added to cart successfully";
				return RedirectToAction(nameof(Index));
            }
			TempData["error"] = response?.Message;
            return View(productDto);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
