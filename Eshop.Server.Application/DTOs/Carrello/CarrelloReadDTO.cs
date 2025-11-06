using Eshop.Server.Dominio.OggettiValore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.DTOs.Carrello
{   public record CarrelloReadDTO(
        int Id,
        int ClienteId,
        List<VoceCarrelloDTO> Voci,
        Money Totale);
}
