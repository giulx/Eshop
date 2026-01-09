export interface RegisterRequest {
  name: string;
  surname: string;
  email: string;
  password: string;

  // indirizzo inviato flat per compatibilità API
  street: string;
  city: string;
  postalCode: string;
  number: string;
}
