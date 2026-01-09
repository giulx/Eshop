export interface UserCreateRequest {
  name: string;
  surname: string;
  email: string;
  password: string;

  street?: string | null;
  city?: string | null;
  postalCode?: string | null;
  number?: string | null;
}
