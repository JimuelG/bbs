export interface BarangayOfficial {
  id: number;
  firstName: string;
  middleName: string;
  lastName: string;
  position: string;
  rank: number;
  isActive: boolean;
  officeImage?: string;
  signatureImage?: string;
}