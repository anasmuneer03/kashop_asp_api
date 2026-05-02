using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using Mapster;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Mapping
{
    public static class MapsterConfig
    {
        public static void MapsterConfigRegister()
        {
            TypeAdapterConfig<Category, CategoryResponse>.NewConfig().
                Map(dest => dest.CategoryId , source => source.Id)
                .Map(dest => dest.UserCreated, source => source.CreatedBy.UserName)
                .Map(dest => dest.Name, source => source.Translations
                    .Where(t=>t.Language == CultureInfo.CurrentCulture.Name)
                        .Select(t => t.Name).FirstOrDefault());

            TypeAdapterConfig<Product, ProductResponse>.NewConfig().
                Map(dest => dest.ProductId, source => source.Id)
                .Map(dest => dest.UserCreated, source => source.CreatedBy.UserName)
                .Map(dest => dest.Name, source => source.Translations
                    .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                    .Select(t => t.Name).FirstOrDefault())
                .Map(dest => dest.MainImage, source => $"http://localhost:5256/images/{source.MainImage}");

            TypeAdapterConfig<ProductUpdateRequest, Product>.NewConfig().
                IgnoreNullValues(true);

            TypeAdapterConfig<Brand, BrandResponse>.NewConfig()
                .Map(dest => dest.UserCreated, source => source.CreatedBy.UserName)
                .Map(dest => dest.Name, src => src.Translations
                    .Where(t => t.Language == CultureInfo.CurrentCulture.Name)
                        .Select(t => t.Name).FirstOrDefault())
                .Map(dest => dest.BrandLogo, src => $"http://localhost:5256/images/{src.BrandLogo}");
        }
    }
}
