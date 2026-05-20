using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        public CartService(ICartRepository cartRepository, IProductRepository productRepository) 
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<bool> AddToCart(AddToCartRequest request, string userId)
        {
            var product = await _productRepository.GetOne(p => p.Id == request.ProductId);
            if (product is null) { return false; }

            var existingItem = await _cartRepository.GetOne(c => c.ProductId == request.ProductId
            && c.UserId == userId);

            var currentCount = existingItem?.Count ?? 0;
            var newCount = currentCount + request.Count;
     
            if(newCount > product.Quantity)
                return false;

            if (existingItem != null ) 
            {
                existingItem.Count = newCount;
                await _cartRepository.UpdateAsync(existingItem);
            }
            else
            {
                    var cartItem = request.Adapt<Cart>();
                    cartItem.UserId = userId;
                    await _cartRepository.CreateAsync(cartItem);
            }

            return true;
        }

        public async Task<List<CartResponse>> GetCart(string userId)
        {
            var cart = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId,
                includes: new string[] {
                    nameof(Cart.Product),
                    $"{nameof(Cart.Product)}.{nameof(Product.Translations)}" 
                });

            return cart.Adapt<List<CartResponse>>();
        }


        public async Task<bool> UpdateQuantity(string userId, int count, int productId)
        {
            var item = await _cartRepository.GetOne(
                filter: c => c.ProductId == productId && c.UserId == userId);

            if (item == null) return false;
            var product = await _productRepository.GetOne(
                filter: p => p.Id == productId);
            
            if (count > product.Quantity) { return false; }
            item.Count = count;

            return await _cartRepository.UpdateAsync(item);

        }

        public async Task<bool> RemoveItem(int productId, string userId)
        {
            var cart = await _cartRepository.GetOne(
                c => c.ProductId == productId && c.UserId == userId);
            if(cart is null) return false;

            return await _cartRepository.DeleteAsync(cart);
        }

        public async Task<bool> ClearCart(string userId)
        {
            var carts = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId);

            if(!carts.Any())
                return false;
   
            return await _cartRepository.DeleteRangeAsync(carts);
        }
    }
}
