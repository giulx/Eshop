import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';

import { AdminOrdersVm } from '../../viewmodels/admin-orders.vm';

@Component({
  selector: 'app-admin-orders-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './admin-orders-page.component.html',
  styleUrls: ['./admin-orders-page.component.css'],
  providers: [AdminOrdersVm],
})
export class AdminOrdersPageComponent {
  readonly vm = inject(AdminOrdersVm);

  constructor() {
    this.vm.init();
  }
}
