
using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
   

        public ProductController(IProductService productService)
        {
            _productService = productService;
            
        }

        public async Task<IActionResult> ProductIndex()
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

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        [ActionName("ProductCreate")]
        public async Task<IActionResult> ProductPost(ProductDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseDto? response = await _productService.CreateProductAsync(model);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["Success"] = "Product created successfully.";
                        return RedirectToAction(nameof(ProductIndex));
                    }
                    TempData["error"] = response.Message;
                }
                TempData["error"] = "Invalid input data.";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return View(model);
            }
        }

        [HttpDelete("{id}")]
        [Route("Product/Delete/{id}")]
        public async Task<IActionResult> ProductDelete(int id)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(id);
            if (response.IsSuccess)
            {
                return Json(new { isSuccess = true ,message= "Product deleted successfully." });
            }
            return Json(new { isSuccess = false, message = response?.Message });
        }

        [HttpGet]
		public async Task<IActionResult> ProductUpdate(int id)
		{
            var response = await _productService.GetProductByIdAsync(id);
            if (response.Result== null)
            {
                TempData["error"] = "Product not found";
                return RedirectToAction(nameof(ProductIndex));

            }
            ProductDto productDto = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
			return View(productDto);
		}

		[HttpPost]
		public async Task<IActionResult> ProductUpdate(ProductDto model)
		{
			try
			{
				if (ModelState.IsValid)
				{
					ResponseDto? response = await _productService.UpdateProductAsync(model);
					if (response != null && response.IsSuccess)
					{
						TempData["Success"] = "Product Updated successfully.";
						return RedirectToAction(nameof(ProductIndex));
					}
					TempData["error"] = response.Message;
				}
				return View(model);
			}
			catch (Exception ex)
			{
				TempData["error"] = ex.Message;
				return View(model);
			}
		}
	}
}
