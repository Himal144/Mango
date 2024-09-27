using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
   
    public class ProductAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private IMapper _mapper;
        private ResponseDto _response;

        public ProductAPIController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();

        }
        [HttpGet]
        public ResponseDto GetAll()
        {
            try
            {
                IEnumerable<Product> productObjList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(productObjList);
            }

            catch (Exception ex) { 
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }



        [HttpGet("{id}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product productObj = _db.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(productObj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }



        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto InsertProduct([FromBody] ProductDto productDto)
        {
            try
            {
                Product productObj = _mapper.Map<Product>(productDto);
                _db.Products.Add(productObj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(productObj);
            }
            catch (Exception ex) { 
                _response.Message=ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPut]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto UpdateProduct([FromBody] ProductDto productDto)
        {
            try
            {
                Product productObj = _mapper.Map<Product>(productDto);
                _db.Products.Update(productObj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(productObj);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpDelete("{id}")]
		[Authorize(Roles = "ADMIN")]
		public ResponseDto DeleteProduct(int id)
        {
            try
            {

                Product productObj = _db.Products.FirstOrDefault(u => u.ProductId == id);
                _db.Products.Remove(productObj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}
