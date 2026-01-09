import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';

import { MyOrdersVm } from '../../viewmodels/my-orders.vm';

@Component({
  selector: 'app-my-orders-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './my-orders-page.component.html',
  styleUrls: ['./my-orders-page.component.css'],
  providers: [MyOrdersVm],
})
export class MyOrdersPageComponent {
  readonly vm = inject(MyOrdersVm);

  constructor() {
    this.vm.init();
  }
}
