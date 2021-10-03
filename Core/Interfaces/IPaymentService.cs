using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IPaymentService
    {
         Task<CustomerShoppingCart> CreateOrUpdatePaymentIntent(string cartId);
    }
}