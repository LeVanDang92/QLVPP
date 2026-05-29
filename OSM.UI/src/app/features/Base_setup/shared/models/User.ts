export interface User {
  userId: string;
  fullName: string | null;
  userName: string;
  email: string;
  passwordShow: string | null;
  department: string;
  isActive: boolean;
  role: string | null;
  createdAt : Date | null;
  createdBy : string | null;
  modifiedAt : Date | null;
  modifiedBy : string| null;
}
