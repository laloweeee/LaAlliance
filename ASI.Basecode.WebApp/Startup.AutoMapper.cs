using AutoMapper;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.ServiceModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using CustomerModels = ASI.Basecode.WebApp.Areas.Customer.Models;

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
                    .ForMember(dest => dest.CartItemOptions, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore())
                    .ForMember(dest => dest.ProductOptionItems, opt => opt.MapFrom(src => src.ProductOptionItems));

                // ProductOptionItems mappings
                CreateMap<ProductOptionItems, ProductOptionItemViewModel>()
                    .ReverseMap()
                    .ForMember(dest => dest.ProductOptionGroup, opt => opt.Ignore())
                    .ForMember(dest => dest.CartItemOptions, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore());

                // Add mapping for ProductOptionItemServiceModel if needed
                CreateMap<ProductOptionItems, ProductOptionItemViewModel>()
                    .ReverseMap()
                    .ForMember(dest => dest.ProductOptionGroup, opt => opt.Ignore())
                    .ForMember(dest => dest.CartItemOptions, opt => opt.Ignore())
                    .ForMember(dest => dest.OrderItemOption, opt => opt.Ignore());

                // Restaurant profile mappings
                CreateMap<Restaurant, RestaurantViewModel>()
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => 
                        src.RestaurantAddress != null && src.RestaurantAddress.Address != null 
                            ? src.RestaurantAddress.Address 
                            : null))
                    .ReverseMap()
                    .ForMember(dest => dest.RestaurantAddress, opt => opt.Ignore());

                // Address mappings
                CreateMap<Address, AddressViewModel>()
                    .ReverseMap();

                CreateMap<UserProfile, UserProfileServiceModel>().ReverseMap();
                
                // UserAddress mapping - handle nested Address entity
                CreateMap<UserAddress, UserAddressServiceModel>()
                    .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                    .ForMember(dest => dest.Barangay, opt => opt.MapFrom(src => src.Address.Barangay))
                    .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                    .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Address.Province))
                    .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.Address.ZipCode.ToString()))
                    .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Address.Longitude))
                    .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Address.Latitude))
                    .ReverseMap()
                    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                    {
                        Street = src.Street,
                        Barangay = src.Barangay,
                        City = src.City,
                        Province = src.Province,
                        ZipCode = string.IsNullOrEmpty(src.ZipCode) ? 0 : int.Parse(src.ZipCode),
                        Longitude = src.Longitude,
                        Latitude = src.Latitude
                    }));

                // UserAddressServiceModel to UserAddressViewModel mapping
                CreateMap<UserAddressServiceModel, CustomerModels.UserAddressViewModel>().ReverseMap();
            }
        }
    }
}
