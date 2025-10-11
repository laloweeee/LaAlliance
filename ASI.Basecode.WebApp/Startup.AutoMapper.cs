using AutoMapper;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.DependencyInjection;

namespace ASI.Basecode.WebApp
{
    // AutoMapper configuration
    internal partial class StartupConfigurer
    {
        /// <summary>
        /// Configure auto mapper
        /// </summary>
        private void ConfigureAutoMapper()
        {
            var mapperConfiguration = new MapperConfiguration(config =>
            {
                config.AddProfile(new AutoMapperProfileConfiguration());
            });

            this._services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
        }

        private class AutoMapperProfileConfiguration : Profile
        {
            public AutoMapperProfileConfiguration()
            {
                CreateMap<UserViewModel, User>();
                CreateMap<ProductCategory, CategoryViewModel>().ReverseMap();
                
                // Product mappings
                CreateMap<Product, ProductViewModel>()
                    .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
                    .ForMember(dest => dest.CustomizationGroups, opt => opt.MapFrom(src => src.ProductOptionGroup))
                    .ReverseMap()
                    .ForMember(dest => dest.ProductCategory, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItems, opt => opt.Ignore())
                    .ForMember(dest => dest.PromotionProducts, opt => opt.Ignore())
                    .ForMember(dest => dest.CustomerProductFavorites, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductOptionGroup, opt => opt.MapFrom(src => src.CustomizationGroups));

                // ProductOptionGroup mappings
                CreateMap<ProductOptionGroup, ProductOptionGroupViewModel>()
                    .ForMember(dest => dest.ProductOptionItems, opt => opt.MapFrom(src => src.ProductOptionItems))
                    .ReverseMap()
                    .ForMember(dest => dest.Product, opt => opt.Ignore())
                    .ForMember(dest => dest.CartItemOption, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductOptionItems, opt => opt.MapFrom(src => src.ProductOptionItems));

                // ProductOptionItems mappings
                CreateMap<ProductOptionItems, ProductOptionItemViewModel>()
                    .ReverseMap()
                    .ForMember(dest => dest.ProductOptionGroup, opt => opt.Ignore())
                    .ForMember(dest => dest.CartItemOption, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore());

                // Add mapping for ProductOptionItemViewModel if needed
                CreateMap<ProductOptionItems, ProductOptionItemViewModel>()
                    .ReverseMap()
                    .ForMember(dest => dest.ProductOptionGroup, opt => opt.Ignore())
                    .ForMember(dest => dest.CartItemOption, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore());
            }
        }
    }
}
