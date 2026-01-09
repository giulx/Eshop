import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';

import { OrderDetailVm } from '../../viewmodels/order-detail.vm';

@Component({
  selector: 'app-order-detail-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './order-detail-page.component.html',
  styleUrls: ['./order-detail-page.component.css'],
  providers: [OrderDetailVm],
})
export class OrderDetailPageComponent {
  readonly vm = inject(OrderDetailVm);

  constructor() {
    this.vm.init();
  }
}
