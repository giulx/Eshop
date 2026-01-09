import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

import { AdminProductListViewModel } from '../../../viewmodels/admin-product-list.vm';

@Component({
  selector: 'app-admin-product-list-page',
  standalone: true,
  imports: [CommonModule, RouterModule],
  providers: [AdminProductListViewModel],
  templateUrl: './admin-product-list-page.component.html',
  styleUrls: ['./admin-product-list-page.component.css'],
})
export class AdminProductListPageComponent {
  readonly vm = inject(AdminProductListViewModel);
  private readonly router = inject(Router);

  // stato modal conferma eliminazione
  isDeleteModalOpen = false;
  productIdToDelete: number | null = null;

  constructor() {
    this.vm.init();
  }

  goToCreate(): void {
    this.router.navigate(['/admin/prodotti/nuovo']);
  }

  goToEdit(id: number): void {
    this.router.navigate(['/admin/prodotti', id, 'modifica']);
  }

  openDeleteModal(id: number): void {
    this.productIdToDelete = id;
    this.isDeleteModalOpen = true;
  }

  closeDeleteModal(): void {
    this.isDeleteModalOpen = false;
    this.productIdToDelete = null;
  }

  confirmDelete(): void {
    if (this.productIdToDelete == null) return;

    this.vm.deleteProduct(this.productIdToDelete);
    this.closeDeleteModal();
  }
}
