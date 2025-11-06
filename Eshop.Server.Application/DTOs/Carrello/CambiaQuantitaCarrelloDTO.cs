using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace Eshop.Server.Applicazione.DTOs.Carrello
{
    public class CambiaQuantitaCarrelloDTO
    {
        [Required]
        public int ProdottoId { get; set; }

        [Range(1, 99)]
        public int Quantita { get; set; }
    }
}

