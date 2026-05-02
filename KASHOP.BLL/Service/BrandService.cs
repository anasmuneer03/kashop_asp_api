using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IFileService _fileService;

        public BrandService(IBrandRepository brandRepository, IFileService fileService)
        {
            _brandRepository = brandRepository;
            _fileService = fileService;
        }
        public async Task CreateBrand(BrandRequest request)
        {
            var brand = request.Adapt<Brand>();
            {
                if (request.BrandLogo != null)
                {
                    var imagePath = await _fileService.UploadAsync(request.BrandLogo);
                    brand.BrandLogo = imagePath;
                }
                await _brandRepository.CreateAsync(brand);
            }
        }
        public async Task<List<BrandResponse>> GetAllBrands()
        {
            var brands = await _brandRepository.GetAllAsync(null,new string[] {nameof(Brand.Translations), nameof(Brand.CreatedBy)});
            var response = brands.Adapt<List<BrandResponse>>();
            return response;
        }

        public async Task<BrandResponse?> GetBrand(Expression<Func<Brand,bool>> filter)
        {
            var brand = await _brandRepository.GetOne(filter,
                new string[] { 
                    nameof(Brand.Translations), 
                    nameof(Brand.CreatedBy) 
                });
            if (brand == null) return null;
            var response = brand.Adapt<BrandResponse>();
            return response;
        }

        public async Task<bool> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetOne(b =>  b.Id == id);
            if (brand == null) return false;
            _fileService.DeleteAsync(brand.BrandLogo);
            return await _brandRepository.DeleteAsync(brand);
        }


    }
}
