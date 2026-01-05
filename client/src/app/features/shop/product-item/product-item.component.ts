import { Component, inject, Input } from '@angular/core';
import { Product } from '../../../shared/models/product';
import { MatCardModule } from '@angular/material/card';
import { CurrencyPipe, CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-product-item',
  standalone: true,
  imports: [
    CommonModule,      // needed for *ngIf, *ngFor
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    RouterModule,
    CurrencyPipe
  ],
  templateUrl: './product-item.component.html',
  styleUrls: ['./product-item.component.scss'], // ✅ plural
})
export class ProductItemComponent {
  @Input() product!: Product;
   cartService = inject(CartService);

  addItemsToCart() {   // ✅ match this name with your template
    console.log('Adding to cart:', this.product);
    this.cartService.addItemToCart(this.product);
  }
}
