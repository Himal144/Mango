using AutoMapper;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.Dto;

namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<CartHeaderDto, CartHeader>();
                config.CreateMap<CartHeader, CartHeaderDto>();
                config.CreateMap<CartDetailsDto, CartDetails>();
                config.CreateMap<CartDetails, CartDetailsDto>();
            });
            return mappingConfig;
        }
    }
}
