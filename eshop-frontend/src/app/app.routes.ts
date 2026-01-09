import { Routes } from '@angular/router';

/* =========================================================
   PUBLIC – Catalogo prodotti
========================================================= */
import { ProductListPageComponent } from './features/products/views/public/product-list-page/product-list-page.component';
import { ProductDetailPageComponent } from './features/products/views/public/product-detail-page/product-detail-page.component';

/* =========================================================
   PUBLIC – Carrello
========================================================= */
import { CartPageComponent } from './features/cart/views/cart-page/cart-page.component';

/* =========================================================
   AUTH – Autenticazione
========================================================= */
import { LoginPageComponent } from './features/auth/views/login-page/login-page.component';
import { RegisterPageComponent } from './features/auth/views/register-page/register-page.component';

/* =========================================================
   USER – Area cliente
========================================================= */
import { ProfilePageComponent } from './features/users/views/profile-page/profile-page.component';
import { MyOrdersPageComponent } from './features/orders/views/my-orders-page/my-orders-page.component';
import { OrderPreviewPageComponent } from './features/orders/views/order-preview-page/order-preview-page.component';
import { OrderDetailPageComponent } from './features/orders/views/order-detail-page/order-detail-page.component';

/* =========================================================
   ADMIN – Gestione prodotti
========================================================= */
import { AdminProductListPageComponent } from './features/products/views/admin/admin-product-list-page/admin-product-list-page.component';
import { AdminProductEditPageComponent } from './features/products/views/admin/admin-product-edit-page/admin-product-edit-page.component';

/* =========================================================
   ADMIN – Gestione utenti
========================================================= */
import { AdminUserListPageComponent } from './features/users/views/admin/admin-user-list-page/admin-user-list-page.component';
import { AdminUserEditPageComponent } from './features/users/views/admin/admin-user-edit-page/admin-user-edit-page.component';

/* =========================================================
   ADMIN – Gestione ordini
========================================================= */
import { AdminOrdersPageComponent } from './features/orders/views/admin-orders-page/admin-orders-page.component';

/* =========================================================
   GUARDS
========================================================= */
import { adminGuard } from './features/auth/guards/admin.guard';
import { customerGuard } from './features/auth/guards/customer.guard';

/* =========================================================
   ROUTES CONFIGURATION
========================================================= */
export const routes: Routes = [

  /* Redirect iniziale */
  {
    path: '',
    redirectTo: 'prodotti',
    pathMatch: 'full',
  },

  /* =====================================================
     PUBLIC
  ====================================================== */
  {
    path: 'prodotti',
    component: ProductListPageComponent,
  },
  {
    path: 'prodotti/:id',
    component: ProductDetailPageComponent,
  },
  {
    path: 'carrello',
    component: CartPageComponent,
  },

  /* =====================================================
     AUTH
  ====================================================== */
  {
    path: 'login',
    component: LoginPageComponent,
  },
  {
    path: 'registrati',
    component: RegisterPageComponent,
  },

  /* =====================================================
     USER – Area cliente (solo utenti non admin)
  ====================================================== */
  {
    path: 'profilo',
    component: ProfilePageComponent,
    canActivate: [customerGuard],
  },
  {
    path: 'ordini',
    component: MyOrdersPageComponent,
  },
  {
    path: 'ordine/preview',
    component: OrderPreviewPageComponent,
  },
  {
    path: 'ordine/:id',
    component: OrderDetailPageComponent,
  },

  /* =====================================================
     ADMIN – Area amministrativa
  ====================================================== */
  {
    path: 'admin/prodotti',
    component: AdminProductListPageComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/prodotti/nuovo',
    component: AdminProductEditPageComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/prodotti/:id/modifica',
    component: AdminProductEditPageComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/utenti',
    component: AdminUserListPageComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/utenti/:id/modifica',
    component: AdminUserEditPageComponent,
    canActivate: [adminGuard],
  },
  {
    path: 'admin/ordini',
    component: AdminOrdersPageComponent,
    canActivate: [adminGuard],
  },

  /* =====================================================
     FALLBACK
  ====================================================== */
  {
    path: '**',
    redirectTo: 'prodotti',
  },
];
