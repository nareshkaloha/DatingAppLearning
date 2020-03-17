import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { MembersListResolver } from './_resolvers/members-list.resolver';
import { PreventUnsavedChanges } from './_guards/prevent-unsavedchanges.gurad';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
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
    //{path: 'messages' , component: MessagesComponent, canActivate: [AuthGuard]},
    {path: 'members',
        component: MemberListComponent,
        canActivate: [AuthGuard],
        resolve: {users: MembersResolver}
    },
    {path: 'members/:id' ,
        component: MemberDetailComponent,
        canActivate: [AuthGuard],
        resolve: { user: MemberDetailResolver}
    },
    {path: 'messages' ,
        component: MessagesComponent,
        canActivate: [AuthGuard],
        resolve: { messages: MessagesResolver}
    },
    {
        path: 'member/edit',
        component: MemberEditComponent,
        canActivate: [AuthGuard],
        resolve: { user: MemberEditResolver },
        canDeactivate: [PreventUnsavedChanges]
    },
    {path: 'lists',
        component: ListsComponent,
        canActivate: [AuthGuard],
        resolve: {users: MembersListResolver}
    },
    {path: 'admin',
        component: AdminPanelComponent,
        canActivate: [AuthGuard],
        data: {roles: ['Admin', 'Moderator']}
    },
    {path: '**', redirectTo: 'home', pathMatch: 'full'}
];
