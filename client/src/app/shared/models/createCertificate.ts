export interface CreateCertificate {
    fullName: string;
    address: string;
    birthDate?: string;
    age?: number;
    certificateType: number;
    purpose: string;
    fee: number;
    civilStatus: string;
    stayDuration: string;
    purok: string;
}