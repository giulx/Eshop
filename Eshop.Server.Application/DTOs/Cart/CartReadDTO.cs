using Eshop.Server.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Cart
{   public record CartReadDTO(
        int Id,
        int CustomerId,
        List<CartItemDTO> Items,
        Money Total);
}
