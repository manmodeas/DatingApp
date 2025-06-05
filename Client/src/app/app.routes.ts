import { Routes } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { MemberListComponent } from './components/member/member-list/member-list.component';
import { MemberDetailComponent } from './components/member/member-detail/member-detail.component';
import { ListsComponent } from './components/lists/lists.component';
import { MessagesComponent } from './components/messages/messages.component';
import { authGuard } from './_guards/auth.guard';
import { MemberEditComponent } from './components/member/member-edit/member-edit.component';
import { preventUnsavedChangesGuard } from './_guards/prevent-unsaved-changes.guard';
import { memberDetailedResolver } from './_resolvers/member-detailed.resolver';
import { AdminPanelComponent } from './components/admin/admin-panel/admin-panel.component';
import { adminGuard } from './_guards/admin.guard';
import { LoginFormComponent } from './components/login-form/login-form.component';
//we can provide multiple gurads in canAvticate 
export const routes: Routes = [
    {path: '', component:HomeComponent},
    {path: 'loginform', component:LoginFormComponent},
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [authGuard],
        canDeactivate: [preventUnsavedChangesGuard],
        children : [
            {path: 'members', component:MemberListComponent},
            {path: 'members/:username', component:MemberDetailComponent, 
                resolve: {member: memberDetailedResolver}},            //when our members/:username gets activated... our resolver get excuted 
            {path: 'member/edit', component:MemberEditComponent},
            {path: 'lists', component:ListsComponent},
            {path: 'messages', component:MessagesComponent},
            {path: 'admin', component:AdminPanelComponent, canActivate: [adminGuard]}
        ]
    },
    {path: '**', component:HomeComponent, pathMatch: 'full'}
];
 