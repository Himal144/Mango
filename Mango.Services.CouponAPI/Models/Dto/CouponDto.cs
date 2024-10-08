﻿namespace Mango.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }

        public double DiscountAmount { get; set; } = 0;

        public double MinimumAmount { get; set; }
    }
}
