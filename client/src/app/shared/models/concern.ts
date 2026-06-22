export interface Concern {
    id: number;
    title: string;
    description: string;
    type: string;
    status: string;
    incidentLocation?: string;
    purok?: string;
    priority: string;
    photoUrl?: string;
    dateReported: string;
    dateResolved?: string;
    resolutionRemarks?: string;
    residentId: number;
    reporterName: string;
    assignedOfficialId?: number;
    assignedOfficialName?: string;

}