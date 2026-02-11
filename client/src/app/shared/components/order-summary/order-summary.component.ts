import { Component, inject } from '@angular/core';
import { CommonModule, Location } from '@angular/common'; // ✅ add Location here
import { RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { CartService } from '../../../core/services/cart.service';

@Component({
  selector: 'app-order-summary',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule
  ],
  templateUrl: './order-summary.component.html',
  styleUrls: ['./order-summary.component.scss'], // ✅ should be styleUrls (plural)
})
export class OrderSummaryComponent {
  cartService = inject(CartService);
  location = inject(Location); // ✅ now works
}