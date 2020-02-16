import { Photo } from './photo';

export interface User {
    id: number;
    username: string;
    gender: string;
    age: number;
    knownAs: string;
    createDate: Date;
    lastActiveDate: Date;
    city: string;
    country: string;
    photoUrl: string;
    interests?: string;
    lookingFor?: string;
    introduction?: string;
    photos: Photo[];
}
