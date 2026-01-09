using Eshop.Server.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Cart
{
    public record CartItemDTO(
        int ProductId,
        string Name,
        int Quantity,
        Money UnitPriceSnapshot,
        Money Subtotal);
}
