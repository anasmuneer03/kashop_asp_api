using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileService _fileService;
        public ProductService(IProductRepository productRepository, 
            IFileService fileService) 
        { 
            _productRepository = productRepository;
            _fileService = fileService;
        }

        public async Task CreateProduct(ProductRequest request)
        {
            var product = request.Adapt<Product>();
            if (request.MainImage != null) 
            {
                var imagePath = await _fileService.UploadAsync(request.MainImage);
                product.MainImage = imagePath;
            }
            await _productRepository.CreateAsync(product);
        }

        public async Task<List<ProductResponse>> GetAllProducts() 
        {
            var products = await _productRepository.GetAllAsync(new string[] {nameof(Product.Translations), nameof(Product.CreatedBy) });
            var response = products.Adapt<List<ProductResponse>>();
            return response;

        }
        public async Task<ProductResponse?> GetProduct(Expression<Func<Product,bool>> filter)
        {
            var product = await _productRepository.GetOne(filter, new string[]
                { 
                  nameof(Product.Translations), 
                  nameof(Product.CreatedBy) 
                });

            if (product == null) return null;

            return product.Adapt<ProductResponse>();
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _productRepository.GetOne(p => p.Id == id);
            if (product == null) return false;
            _fileService.DeleteAsync(product.MainImage);
            return await _productRepository.DeleteAsync(product);

        }
    }
}
