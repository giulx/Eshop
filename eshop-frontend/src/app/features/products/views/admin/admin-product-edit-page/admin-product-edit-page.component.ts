import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { filter, take } from 'rxjs';

import { AdminProductEditViewModel } from '../../../viewmodels/admin-product-edit.vm';

interface ProductFormModel {
  name: string;
  description: string | null;
  price: number;
  currency: string;
  availableQuantity: number;
}

@Component({
  selector: 'app-admin-product-edit-page',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  providers: [AdminProductEditViewModel],
  templateUrl: './admin-product-edit-page.component.html',
  styleUrls: ['./admin-product-edit-page.component.css'],
})
export class AdminProductEditPageComponent {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  readonly vm = inject(AdminProductEditViewModel);

  formModel: ProductFormModel = {
    name: '',
    description: null,
    price: 0,
    currency: 'EUR',
    availableQuantity: 0,
  };

  isEditMode = false;

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');

    if (idParam) {
      const id = Number(idParam);

      if (isNaN(id)) {
        this.vm.error$.next('ID prodotto non valido.');
        this.router.navigate(['/admin/prodotti']);
        return;
      }

      this.isEditMode = true;
      this.vm.initForEdit(id);

      // Aspetta il primo valore NON nullo (quello caricato dal server)
      this.vm.product$
        .pipe(
          filter((product): product is NonNullable<typeof product> => product !== null),
          take(1)
        )
        .subscribe((product) => {
          this.formModel = {
            name: product.name,
            description: product.description ?? null,
            price: product.price,
            currency: product.currency,
            availableQuantity: product.availableQuantity,
          };
        });

      return;
    }

    this.isEditMode = false;
    this.vm.initForCreate();
  }

  onSubmit(): void {
    this.vm.save(this.formModel);
  }

  goBack(): void {
    this.router.navigate(['/admin/prodotti']);
  }
}
