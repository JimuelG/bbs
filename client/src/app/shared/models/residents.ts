export interface Resident {
    id: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    purok: string;
    monthlyIncome: number;
    appUserId: string;
    email: string;
    idUrl: string;
    pictureUrl: string;
    isIdVerified: boolean;
    role: string;
}