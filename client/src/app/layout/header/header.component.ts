import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { RouterLink, RouterLinkActive } from "@angular/router";




@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatIconModule,
    MatBadgeModule,
    MatButtonModule,
    RouterLink,
    RouterLinkActive
],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'], // ✅ plural
})
export class HeaderComponent {

}