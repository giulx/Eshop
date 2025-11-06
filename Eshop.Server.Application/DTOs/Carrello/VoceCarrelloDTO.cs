using Eshop.Server.Domain.OggettiValore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.DTOs.Carrello
{
    public record VoceCarrelloDTO(
        int ProdottoId,
        string Nome,
        int Quantita,
        Money PrezzoUnitarioSnapshot,
        Money Subtotale);
}
