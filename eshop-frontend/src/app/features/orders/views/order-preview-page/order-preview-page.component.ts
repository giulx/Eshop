import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { OrderPreviewVm } from '../../viewmodels/order-preview.vm';

@Component({
  selector: 'app-order-preview-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './order-preview-page.component.html',
  styleUrls: ['./order-preview-page.component.css'],
  providers: [OrderPreviewVm],
})
export class OrderPreviewPageComponent {
  readonly vm = inject(OrderPreviewVm);

  constructor() {
    this.vm.init();
  }
}
