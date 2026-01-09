import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { AdminUserEditVm } from '../../../viewmodels/admin-user-edit.vm';

@Component({
  selector: 'app-admin-user-edit-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './admin-user-edit-page.component.html',
  styleUrls: ['./admin-user-edit-page.component.css'],
  providers: [AdminUserEditVm],
})
export class AdminUserEditPageComponent implements OnInit {
  readonly vm = inject(AdminUserEditVm);

  ngOnInit(): void {
    this.vm.init();
  }

  onSubmit(): void {
    this.vm.save(this.vm.formModel());
  }
}
