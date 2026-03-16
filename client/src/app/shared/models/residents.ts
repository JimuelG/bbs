export interface Resident {
    id: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    birthDate: string;
    phoneNumber: number;
    purok: string;
    monthlyIncome: number;
    appUserId: string;
    email: string;
    idUrl: string;
    pictureUrl: string;
    isIdVerified: boolean;
    role: string;
}