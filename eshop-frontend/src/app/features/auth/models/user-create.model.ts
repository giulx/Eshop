export interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  password: string;

  street: string;
  city: string;
  postalCode: string;
  number: string;
}

/**
 * Alias intenzionale: stesso payload per registrazione e creazione utente.
 * Evita divergenze tra modelli lato client.
 */
export type UserCreateModel = RegisterRequest;
