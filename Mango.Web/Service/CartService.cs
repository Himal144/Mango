using Mango.Web.Models;
using Mango.Web.Models.CartAPIDto;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    
    public class CartService : ICartService
    {
        public IBaseService _baseService { get; set; }
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                apiType = SD.ApiType.POST,
                Data =cartDto,
                Url = SD.CartAPIBase + "/api/cart/ApplyCoupon" 
            });
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                apiType = SD.ApiType.GET,
                Url = SD.CartAPIBase + "/api/cart/GetCart/" + userId
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                apiType = SD.ApiType.DELETE,
                Data = cartDetailsId,
                Url = SD.CartAPIBase + "/api/cart/RemoveFromCart"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                apiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.CartAPIBase + "/api/cart/"
            });
        }
    }
}
