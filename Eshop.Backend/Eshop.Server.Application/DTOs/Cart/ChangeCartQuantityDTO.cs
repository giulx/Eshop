using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Application.DTOs.Cart
{
    public class ChangeCartQuantityDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Range(1, 99)]
        public int Quantity { get; set; }
    }
}

