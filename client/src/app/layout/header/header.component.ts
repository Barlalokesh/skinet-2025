import { Component } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';




@Component({
  selector: 'app-header',
  standalone: true,
  imports: [
    MatIconModule, 
    MatBadgeModule, 
    MatButtonModule
    
  ],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'], // ✅ plural
})
export class HeaderComponent {

}