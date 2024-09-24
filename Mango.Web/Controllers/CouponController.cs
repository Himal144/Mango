using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Mango.Web.Controllers
{

    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
            _couponService = couponService;
        }
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto> list = new();
            ResponseDto? response = await _couponService.GetAllCouponsAsync();
            if (response != null && response.IsSuccess) {
                list = JsonConvert.DeserializeObject<List<CouponDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return View(list);
        }


        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        [ActionName("CouponCreate")]
        public async Task<IActionResult> CouponPost(CouponDto model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ResponseDto? response = await _couponService.CreateCouponAsync(model);
                    if (response != null && response.IsSuccess)
                    {
                        TempData["Success"] = "Coupon created successfully.";
                        return RedirectToAction(nameof(CouponIndex));
                    }
                    TempData["error"] = response.Message;
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["error"]=ex.Message;
                return View(model);
            }
        }

        [HttpDelete("{id}")]
        [Route("Coupon/Delete/{id}")]
        public async Task<IActionResult> CouponDelete(int id)
        {
                ResponseDto? response = await _couponService.DeleteCouponAsync(id);
                if (response.IsSuccess)
                {
                    return Json( new { IsSuccess=true,Message="Coupon Deleted Successfully"});
                }
                return Json(new {IsSuccess=false,Message=response?.Message });  
        }

    }
}
