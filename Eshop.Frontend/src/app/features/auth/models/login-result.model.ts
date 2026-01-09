export interface LoginResult {
  success: boolean;
  message: string;

  /**
   * Codice errore tecnico valorizzato solo in caso di fallimento.
   * Serve per distinguere errori funzionali lato client.
   */
  errorCode?: string | null;

  // valorizzati solo se success === true
  userId: number | null;
  name: string | null;
  surname: string | null;

  isAdmin: boolean;
  token: string | null;
}
