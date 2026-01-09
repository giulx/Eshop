export interface UserModel {
  id: number;
  name: string;
  surname: string;
  email: string;

  street: string | null;
  city: string | null;
  postalCode: string | null;
  number: string | null;

  isAdmin: boolean;
}