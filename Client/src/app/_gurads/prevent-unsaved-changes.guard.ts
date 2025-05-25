import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../components/member/member-edit/member-edit.component';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  if(component.editForm?.dirty)
  {
    return confirm('Are you sure you want to continue? Any unsaved chnages will be lost')
  }
  return true;
};
