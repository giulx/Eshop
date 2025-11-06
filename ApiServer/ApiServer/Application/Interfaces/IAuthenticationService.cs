using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces
{
    /// <summary>
    /// Interfaccia che definisce i contratti per l'autenticazione e la registrazione degli utenti.
    /// Fa parte dello strato Application e viene implementata dal servizio concreto AuthenticationService.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Registra un nuovo utente nel sistema.
        /// </summary>
        /// <param name="nome">Nome dell'utente.</param>
        /// <param name="cognome">Cognome dell'utente.</param>
        /// <param name="email">Indirizzo email univoco dell'utente.</param>
        /// <param name="password">Password in chiaro da hashare.</param>
        /// <returns>
        /// Una tupla contenente:
        /// - Success: esito dell'operazione (true se registrato, false se errore)
        /// - Message: eventuale messaggio descrittivo.
        /// </returns>
        Task<(bool Success, string Message)> RegisterAsync(string nome, string cognome, string email, string password);

        /// <summary>
        /// Effettua il login di un utente esistente e restituisce un token JWT se le credenziali sono corrette.
        /// </summary>
        /// <param name="email">Email dell'utente.</param>
        /// <param name="password">Password in chiaro da verificare.</param>
        /// <returns>
        /// Una tupla contenente:
        /// - Success: esito dell’operazione
        /// - Token: JWT generato in caso di successo
        /// - Message: messaggio informativo o di errore
        /// </returns>
        Task<(bool Success, string? Token, string? Message)> LoginAsync(string email, string password);
    }
}
