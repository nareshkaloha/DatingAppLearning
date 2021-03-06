export interface Photo {
    id: number;
    url: string;
    description: string;
    createdDate: Date;
    isMain: boolean;
    userName?: string;
    isApproved?: boolean;
}
