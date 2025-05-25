import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MemberListComponent } from './components/member/member-list/member-list.component';
import { MemberDetailComponent } from './components/member/member-detail/member-detail.component';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { authGuard } from './_gurads/auth.guard';
//we can provide multiple gurads in canAvticate 
export const routes: Routes = [
    {path: '', component:HomeComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        children : [
            {path: 'members', component:MemberListComponent},
            {path: 'members/:username', component:MemberDetailComponent},
            {path: 'lists', component:ListsComponent},
            {path: 'messages', component:MessagesComponent}
        ]
    },
    {path: '**', component:HomeComponent, pathMatch: 'full'}
];
 