export interface Announcement {
    id: number;
    title: string;
    message: string;
    languageCode: string;
    audioUrl: string;
    scheduledAt: string;
    expireAt?: string;
    isEmergency: boolean;
    isPlayed: boolean;
}