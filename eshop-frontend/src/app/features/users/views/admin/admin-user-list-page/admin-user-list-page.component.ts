import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { RouterModule } from '@angular/router';

import { AdminUserListVm } from '../../../viewmodels/admin-user-list.vm';

@Component({
  selector: 'app-admin-user-list-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './admin-user-list-page.component.html',
  styleUrls: ['./admin-user-list-page.component.css'],
  providers: [AdminUserListVm],
})
export class AdminUserListPageComponent implements OnInit {
  readonly vm = inject(AdminUserListVm);

  ngOnInit(): void {
    this.vm.init();
  }
}
