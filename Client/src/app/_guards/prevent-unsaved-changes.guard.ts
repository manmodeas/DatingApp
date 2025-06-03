import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../components/member/member-edit/member-edit.component';
import { inject } from '@angular/core';
import { ConfirmService } from '../_services/confirm.service';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService = inject(ConfirmService);

  if(component?.editForm?.dirty)
  {
    // return confirm('Are you sure you want to continue? Any unsaved chnages will be lost')
    return confirmService.confirm() ?? false;
  }
  return true;
};
