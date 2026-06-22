export interface ResidentRecord {
    id?: number;
    type: 'Certificate' | 'Concern',
    title: string,
    description: string,
    referenceNumber?: string,
    status: string;
    createdAt: string;
}