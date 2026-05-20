using KASHOP.DAL.DTO.Request;
using KASHOP.DAL.DTO.Response;
using KASHOP.DAL.Models;
using KASHOP.DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Service
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;
        private readonly IProductRepository _productRepository;
        private readonly IEmailSender _emailSender;

        public CheckoutService(ICartRepository cartRepository,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IOrderRepository orderRepository,
            ICartService cartService,
            IProductRepository productRepository,
            IEmailSender emailSender)
        {
            _cartRepository = cartRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _orderRepository = orderRepository;
            _cartService = cartService;
            _productRepository = productRepository;
            _emailSender = emailSender;
        }

        public async Task<CheckoutResponse> CheckoutProcess(string userId, CheckoutRequest request)
        {
            var cartItems = await _cartRepository.GetAllAsync(
                filter: c => c.UserId == userId
                , includes: new[] { nameof(Cart.Product) ,
                $"{nameof(Cart.Product)}.{nameof(Product.Translations)}" }
                );

            if (!cartItems.Any())
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "Cart is empty"
                };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new CheckoutResponse
            {
                Success = false,
                Error = "User not found"
            };
            var city = request.City ?? user.City;
            var street = request.Street ?? user.Street;
            var phone = request.PhoneNumber ?? user.PhoneNumber;

            if (city is null)
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "City is Required"
                };
            }

            if (street is null)
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "Street is required"
                };
            }

            if (phone is null)
            {
                return new CheckoutResponse
                {
                    Success = false,
                    Error = "phone is required"
                };
            }

            foreach (var item in cartItems)
            {
                if (item.Count > item.Product.Quantity)
                {
                    return new CheckoutResponse
                    {
                        Success = false,
                        Error = "Dosn't have enough stock"
                    };
                }
                //var TotalPrice = item.Count*item.Product.Price;
            }

            var order = new Order
            {
                UserId = userId,
                City = city,
                Street = street,
                PhoneNumber = phone,
                paymentMethod = request.PaymentMethod,
                AmountPaid = cartItems.Sum(c => c.Product.Price * c.Count),
                orderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Count,
                    UnitPrice = c.Product.Price,
                    TotalPrice = c.Product.Price * c.Count
                }).ToList()
            };

            await _orderRepository.CreateAsync(order);

            if (request.PaymentMethod == PaymentMethodEnum.Cash)
            {
                return new CheckoutResponse
                {
                    Success = true,
                    Error = "",
                    OrderId = order.Id
                };
            }
            if (request.PaymentMethod == PaymentMethodEnum.Visa)
            {
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/api/checkout/success?sessionId={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/checkout/cancel",
                    LineItems = new List<SessionLineItemOptions>()
                }; 

                foreach(var item in cartItems)
                {
                    options.LineItems.Add(
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "USD",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = item.Product.Translations.FirstOrDefault(t => t.Language == "en").Name,
                                },
                                UnitAmount = (long)(item.Product.Price*100),
                            },
                            Quantity = item.Count,
                        });
                }
                var service = new SessionService();
                var session = service.Create(options);

                order.StripeSessionId = session.Id;
                await _orderRepository.UpdateAsync(order);

                return new CheckoutResponse
                {
                    Success = true,
                    StripeUrl = session.Url
                };
            }


            return new CheckoutResponse
            {
                Success = false,
                Error = "Invalid payment"
            };
        }

        public async Task<CheckoutResponse> HandleSuccess(string sessionId)
        {
            var order = await _orderRepository.GetOne(
                filter: o => o.StripeSessionId == sessionId,
                includes: new[] {nameof(Order.orderItems), 
                $"{nameof(Order.orderItems)}.{nameof(OrderItem.Product)}",
                $"{nameof(Order.orderItems)}.{nameof(OrderItem.Product)}.{nameof(Product.Translations)}"
                }
                );

            order.orderStatus = OrderStatusEnum.Paid;

            await _orderRepository.UpdateAsync(order);

            await _cartService.ClearCart(order.UserId);

            var user = await _userManager.FindByIdAsync(order.UserId);


            var lowStockProduct = await _productRepository.DecreaseQuantityAsync(order.orderItems);

            await _emailSender.SendEmailAsync(user.Email, "order confirmed", "<h2>your order has been placed successfully</h2>");


            foreach (var item in lowStockProduct)
            {
                if (lowStockProduct != null)
                {
                    await _emailSender.SendEmailAsync(user.Email, "low stock alert", $"<h2>{item.Translations.FirstOrDefault(t => t.Language == "en").Name} current quantity: {item.Quantity}</h2>");
                }
            }

            return new CheckoutResponse
            {
                Success = true,
                OrderId = order.Id
            }; 

        }
    }
}
