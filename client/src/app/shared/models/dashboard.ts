export interface Dashboard {
    stats: DashboardStats;
    recentRequest: RecentCertificatesRequest[];
    recentConcern: UrgentConcern[];
    officials: DashboardOfficial[];
}

export interface DashboardStats {
    totalResidents: number;
    verifiedResidents: number;
    pendingCertificates: number;
    activeConcerns: number;
    activeAnnouncements: number;
}

export interface RecentCertificatesRequest {
    id: number;
    name: string;
    type: string;
    date: string;
    status: string;
}

export interface UrgentConcern {
    id: number;
    title: string;
    purok: string;
    priority: string;
}

export interface DashboardOfficial {
    id: number;
    name: string;
    position: string;
    officeImage?: string;
}