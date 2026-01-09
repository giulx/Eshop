using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Cart
{
    public class AddCartItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; }
    }
}
