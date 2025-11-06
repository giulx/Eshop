using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.DTOs.Carrello
{
    public class AggiungiVoceCarrelloDTO
    {
        [Required]
        public int ProdottoId { get; set; }

        [Range(1, 99)]
        public int Quantita { get; set; }
    }
}
