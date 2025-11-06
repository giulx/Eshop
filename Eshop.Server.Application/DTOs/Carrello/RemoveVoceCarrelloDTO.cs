using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Carrello
{
    public class RimuoviVoceCarrelloDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProdottoId { get; set; }
    }
}

