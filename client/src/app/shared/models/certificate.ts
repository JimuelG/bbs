export interface Certificate {
    id: number;
    referenceNumber: string;
    fullName: string;
    address: string;
    certificateType: number;
    purpose: string;
    fee: string;
    age?: number;
    birthDate?: string;
    civilStatus: string;
    placeOfBirth: string;
    purok: string;
    stayDuration: string;
    requestDate: string;
    issuedAt: string;
    issuedBy: string;
    status: string;
}