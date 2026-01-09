import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { LoginVm } from '../../viewmodels/login.vm';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css'],
  providers: [LoginVm],
})
export class LoginPageComponent {
  readonly vm = inject(LoginVm);

  constructor() {
    this.vm.init();
  }
}
