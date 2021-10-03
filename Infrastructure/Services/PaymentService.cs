using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Orders;
using Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Product = Core.Entities.Product;
using Order = Core.Entities.Orders.Order;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public PaymentService(IShoppingCartRepository cartRepository, 
            IUnitOfWork unitOfWork, 
            IConfiguration configuration)
        {
            _configuration = configuration;
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerShoppingCart> CreateOrUpdatePaymentIntent(string cartId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var cart = await _cartRepository.GetShoppingCartAsync(cartId);
            if(cart == null) return null;

            var shippingPrice = 0m;

            if (cart.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork
                    .Repository<DeliveryMethod>()
                    .GetByIdAsync((int)cart.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }

            foreach (var item in cart.Items)
            {
                var productItem = await _unitOfWork
                    .Repository<Product>()
                    .GetByIdAsync(item.Id);
                if (item.Price != productItem.Price)
                {
                    item.Price = productItem.Price;
                }
            }

            var service = new PaymentIntentService();

            PaymentIntent intent;

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long) cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long) shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> {"card"}
                };
                intent = await service.CreateAsync(options);
                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long) cart.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long) shippingPrice * 100
                };
                await service.UpdateAsync(cart.PaymentIntentId, options);
            }

            await _cartRepository.UpdateShoppingCartAsync(cart);

            return cart;
        }

        public async Task<Core.Entities.Orders.Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentFailed;
            await _unitOfWork.Complete();

            return order;
        }

        public async Task<Core.Entities.Orders.Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (order == null) return null;

            order.Status = OrderStatus.PaymentRecevied;
            _unitOfWork.Repository<Order>().Update(order);

            await _unitOfWork.Complete();

            return order;
        }
    }
}
