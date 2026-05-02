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
            var products = await _productRepository.GetAllAsync(
                p => p.Status == EntityStatus.Active,
                new string[] {nameof(Product.Translations), nameof(Product.CreatedBy) });
            
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
        public async Task<bool> UpdateProduct(int id, ProductUpdateRequest request)
        {
            var productDB = await _productRepository.GetOne(p => p.Id == id,
                new string[] { nameof(Product.Translations) }
                ); 
            if (productDB == null) return false;

            var oldImage = productDB.MainImage;

            request.Adapt<Product>();

            if (request.Translations != null)
            {
                foreach (var translationRequest in request.Translations)
                {
                    var existing = productDB.Translations.FirstOrDefault(t => t.Language == translationRequest.Language);
                    if (existing != null)
                    {
                        if (translationRequest.Name != null)
                        {
                            existing.Name = translationRequest.Name;
                        }
                        if (translationRequest.Description != null)
                        {
                            existing.Description = translationRequest.Description;
                        }
                    }
                    return false;
                }
            }

                if (request.MainImage != null)
                {
                    _fileService.DeleteAsync(oldImage);
                    productDB.MainImage = await _fileService.UploadAsync(request.MainImage);
                }
                productDB.MainImage = oldImage;

                return await _productRepository.UpdateAsync(productDB);
        }

        public async Task<bool> ToggleStatus(int id)
        {
            var product = await _productRepository.GetOne(p => p.Id == id);
            if (product is null) return false;

            product.Status = product.Status == EntityStatus.Active ?
                EntityStatus.InActive : EntityStatus.Active;
            return await _productRepository.UpdateAsync(product);
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
