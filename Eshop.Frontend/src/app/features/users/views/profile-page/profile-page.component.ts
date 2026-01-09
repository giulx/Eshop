import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';

import { ProfileVm } from '../../viewmodels/profile.vm';

@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-page.component.html',
  styleUrls: ['./profile-page.component.css'],
  providers: [ProfileVm],
})
export class ProfilePageComponent implements OnInit {
  readonly vm = inject(ProfileVm);

  ngOnInit(): void {
    this.vm.init();
  }
}
