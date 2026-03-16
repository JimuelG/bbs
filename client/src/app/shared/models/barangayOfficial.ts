export interface BarangayOfficial {
  id: number;
  firstName: string;
  middleName: string;
  lastName: string;
  position: string;
  pictureUrl: string;
  phoneNumber: string;
  purok: string;
  email: string;
  rank: number;
  isActive: boolean;
  officeImage?: string;
  signatureImage?: string;
}