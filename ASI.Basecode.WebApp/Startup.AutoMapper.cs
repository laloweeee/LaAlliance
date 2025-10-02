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
                CreateMap<Category, CategoryViewModel>().ReverseMap();
                // Product mappings
                CreateMap<Product, ProductViewModel>()
                    .ForMember(dest => dest.ImageFile, opt => opt.Ignore())
                    .ForMember(dest => dest.CustomizationGroups, opt => opt.MapFrom(src => src.CustomizationGroups))
                    .ReverseMap()
                    .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                    .ForMember(dest => dest.Category, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedByUser, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedByUser, opt => opt.Ignore());

                // CustomizationGroup mappings
                CreateMap<CustomizationGroup, CustomizationGroupViewModel>()
                    .ForMember(dest => dest.CustomizationOptions, opt => opt.MapFrom(src => src.CustomizationOptions))
                    .ReverseMap()
                    .ForMember(dest => dest.Product, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());

                // CustomizationOption mappings
                CreateMap<CustomizationOption, CustomizationOptionViewModel>()
                    .ReverseMap()
                    .ForMember(dest => dest.CustomizationGroup, opt => opt.Ignore())
                    .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
                    .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore());
            }
        }
    }
}
