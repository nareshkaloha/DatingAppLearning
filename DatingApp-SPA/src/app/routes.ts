import { MembersResolver } from './_resolvers/members.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { User } from './_models/user';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { AuthGuard } from './_guards/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { HomeComponent } from './home/home.component';
import { Routes } from '@angular/router';

export const appRoutes: Routes = [
    {path: 'home' , component: HomeComponent},
    {path: 'messages' , component: MessagesComponent, canActivate: [AuthGuard]},
    {path: 'members' , 
        component: MemberListComponent,
        canActivate: [AuthGuard],
        resolve: {users: MembersResolver}
    },
    {path: 'members/:id' ,
        component: MemberDetailComponent,
        canActivate: [AuthGuard],
        resolve: { user: MemberDetailResolver}
    },
    {path: 'lists' , component: ListsComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: 'home', pathMatch: 'full'}
];
