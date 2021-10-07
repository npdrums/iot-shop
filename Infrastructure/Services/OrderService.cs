using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Orders;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        public OrderService(IShoppingCartRepository cartRepository,
            IUnitOfWork unitOfWork,
            IPaymentService paymentService)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
            _cartRepository = cartRepository;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId,
            string shoppingCartId, Address shippingAddress)
        {
            var shoppingCart = await _cartRepository.GetShoppingCartAsync(shoppingCartId);

            var items = new List<OrderItem>();
            foreach (var item in shoppingCart.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            var subtotal = items.Sum(item => item.Price * item.Quantity);

            var spec = new OrderByPaymentIntentIdSpecification(shoppingCart.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(shoppingCart.PaymentIntentId);
            }

            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal, shoppingCart.PaymentIntentId);
            _unitOfWork.Repository<Order>().Add(order);

            var result = await _unitOfWork.Complete();

            if (result <= 0)
            {
                return null;
            }

            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id, buyerEmail);

            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);

            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}