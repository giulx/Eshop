import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { RouterModule } from '@angular/router';

import { CartVm } from '../../viewmodels/cart.vm';

@Component({
  selector: 'app-cart-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './cart-page.component.html',
  styleUrls: ['./cart-page.component.css'],
  providers: [CartVm],
})
export class CartPageComponent {
  readonly vm = inject(CartVm);

  constructor() {
    this.vm.init();
  }
}
