using AutoMapper;
using Mango.Services.CouponAPI.Models.Dto;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public IMapper _mapper;
        public  ResponseDto _response;
        public IProductService _productService;
        public ICouponService _couponService;

        public ShoppingCartAPIController(ApplicationDbContext db, IMapper mapper,
            IProductService productService,ICouponService couponService)
        {
            _db = db;   
            _mapper = mapper;
            _response = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
        }

        [HttpPost]
        public async  Task<ResponseDto> UpsertCart([FromBody]CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //User is adding the first item in the cart so create the cartHeader first and then add the cart details.
                    //Create cart header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();


                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //If the cart is not null
                    //Check whether the user have the same product already in the cart or not.
                    var cartDetailsDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartDto.CartDetails.First().ProductId
                    && u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsDb == null)
                    {
                        //User have no similar product so create the new cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        CartDetails cartDetails = _mapper.Map<CartDetails>(cartDto.CartDetails.First());
                        await _db.CartDetails.AddAsync(cartDetails);
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        //User already have the similar product in the cart so add count.
                        cartDto.CartDetails.First().Count += cartDetailsDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex) {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            
            }
            return _response;

        }

        [HttpDelete("RemoveFromCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId) 
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails.First(u=> u.CartDetailsId == cartDetailsId);
                int totalcountofcartitem = _db.CartDetails.Where(u=> u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if(totalcountofcartitem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.
                        AsNoTracking().FirstOrDefaultAsync(u=>u.CartHeaderId==cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                _db.SaveChangesAsync();
            }
            catch (Exception ex) { 
            _response.IsSuccess=false;
            _response.Message = ex.Message;
            }
            return _response;
        }



        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_db.CartDetails.
                    Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));
                IEnumerable<ProductDto> productDtos = await _productService.GetProducts();
                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                    {
                    CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinimumAmount) {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.DiscountAmount = coupon.DiscountAmount;
                    }
                }
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost("ApplyCoupon")]

        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartHeaderFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartHeaderFromDb);
                _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
