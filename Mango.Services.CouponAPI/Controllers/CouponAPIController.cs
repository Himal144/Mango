using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDTO _response;
        private IMapper _mapper;
        public CouponAPIController(ApplicationDbContext db,IMapper mapper)
        {
            _db = db;
            _response = new ResponseDTO();
            _mapper = mapper;
        }
        [HttpGet]
      
        public ResponseDTO GetAll()
        {
            try
            {
                IEnumerable<Coupon> couponObjList=_db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<Coupon>>(couponObjList);
            }
            catch (Exception ex) {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
               
            }
            return _response;
        }



        [HttpGet("{id}")]
        public ResponseDTO Get( int id)
        {
            try
            {
                Coupon couponObj = _db.Coupons.First(u=>u.CouponId==id);
                _response.Result = _mapper.Map<Coupon>(couponObj);
                
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
             
            }
            return _response;

        }


        [HttpGet]
        [Route("GetByCode/{code}")]

        public ResponseDTO GetByCode(string code)
        {
            try
            {
                Coupon couponObj = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
                if(couponObj == null)
                {
                    _response.IsSuccess=false;
                }
                _response.Result = _mapper.Map<Coupon>(couponObj);

            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;

            }
            return _response;
        }


        [HttpPost]
        public ResponseDTO InsertCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon CouponObj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(CouponObj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(CouponObj);
            }
            catch (Exception ex) {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return (_response);
        }

        [HttpPut]
        public ResponseDTO UpdateCoupon([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon CouponObj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Update(CouponObj);
                _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(CouponObj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return (_response);
        }

        [HttpDelete("{id}")]
        public ResponseDTO DeleteCoupon(int id)
        {
            try
            {
                Coupon CouponObj = _db.Coupons.First(u => u.CouponId == id);
                _db.Coupons.Remove(CouponObj);
                _db.SaveChanges();  
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return (_response);
        }



        //Uploading the coupon through the excel file

    [HttpPost("upload-excel")]
    public IActionResult UploadCouponExcel(IFormFile file)
    {
        if (file == null || file.Length <= 0)
        {
            return BadRequest("Invalid file.");
        }

        List<CouponDto> couponList = new List<CouponDto>();

        try
        {
                // Set the EPPlus license context to non-commercial
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                // Read the Excel file
                using (var stream = new MemoryStream())
            {
                file.CopyTo(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // Assume data is in the first worksheet
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // Skip header row (assuming the first row is the header)
                    {
                        var couponDto = new CouponDto
                        {
                            CouponCode = worksheet.Cells[row, 1].Value?.ToString().Trim(),
                            DiscountAmount = Convert.ToDouble(worksheet.Cells[row, 2].Value),
                            MinimumAmount = Convert.ToDouble(worksheet.Cells[row, 3].Value)
                        };
                        couponList.Add(couponDto);
                    }
                }
            }

            // Map CouponDto to Coupon and insert data into database
            foreach (var couponDto in couponList)
            {
                Coupon couponObj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(couponObj);
            }
            _db.SaveChanges();

            _response.IsSuccess = true;
            _response.Result = couponList;
        }
        catch (Exception ex)
        {
            _response.IsSuccess = false;
            _response.Message = ex.Message;
        }

        return Ok(_response);
    }


}
}
