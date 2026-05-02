using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public interface IProductService
    {
        Task<List<ProductResponse>> GetAllProducts();
        Task CreateProduct(ProductRequest request);
        Task<ProductResponse?> GetProduct(Expression<Func<Product, bool>> filter);
        Task<bool> UpdateProduct(int id, ProductUpdateRequest request);
        Task<bool> ToggleStatus(int id);
        Task<bool> DeleteProduct(int id);
    }
}
