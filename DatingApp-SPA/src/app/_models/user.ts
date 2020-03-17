import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    gender: string;
    age: number;
    knownAs: string;
    createDate: Date;
    lastActiveDate: any;
    city: string;
    country: string;
    photoUrl: string;
    interests?: string;
    lookingFor?: string;
    introduction?: string;
    photos: Photo[];
    roles?: string[];
}
