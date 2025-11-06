using Eshop.Server.Domain.OggettiValore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Carrello
{   public record CarrelloReadDTO(
        int Id,
        int ClienteId,
        List<VoceCarrelloDTO> Voci,
        Money Totale);
}
