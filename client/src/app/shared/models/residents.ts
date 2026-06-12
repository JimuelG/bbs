export interface Resident {
    id: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    birthDate: string;
    phoneNumber: number;
    purok: string;
    monthlyIncome: number;
    civilStatus: string;
    appUserId: string;
    email: string;
    idUrl: string;
    pictureUrl: string;
    isIdVerified: boolean;
    role: string;
}