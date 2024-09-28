using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async  Task<CouponDto> GetCoupon(string CouponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{CouponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp!= null )
            {
                if (resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));
                }
               
            }
            return new CouponDto();
        }
    }
}
