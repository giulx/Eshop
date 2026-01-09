using Eshop.Server.Domain.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshop.Server.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> PayAsync(int customerId, decimal importo);
        Task<bool> RefundAsync(int customerId, decimal importo);
    }
}
