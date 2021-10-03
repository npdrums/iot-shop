using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Orders;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
         Task<CustomerShoppingCart> CreateOrUpdatePaymentIntent(string cartId);
         Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId);
         Task<Order> UpdateOrderPaymentFailed(string paymentIntentId);
    }
}