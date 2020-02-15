import { AuthGuard } from './_guards/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';

export const appRoutes: Routes = [
    {path: 'home' , component: HomeComponent},
    {path: 'messages' , component: MessagesComponent, canActivate: [AuthGuard]},
    {path: 'members' , component: MemberListComponent, canActivate: [AuthGuard]},
    {path: 'lists' , component: ListsComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: 'home', pathMatch: 'full'}
];
