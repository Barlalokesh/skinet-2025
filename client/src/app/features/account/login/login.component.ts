import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule, MatLabel} from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AccountService } from '../../../core/services/account.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatLabel,
    MatButtonModule
],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss',
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private accountService = inject(AccountService);
  private router = inject (Router);
  private activatedRoute = inject(ActivatedRoute);
  returnUrl = '/shop';

  constructor(){
    const Url =this.activatedRoute.snapshot.queryParams['returnUrl'];
    if(Url) this.returnUrl = Url;
  }

loginForm = this.fb.group({
  email:[''],
  password:['']
});

onSubmit(){
this.accountService.login(this.loginForm.value).subscribe({
  next: () =>{
    this.accountService.getUserInfo().subscribe();
    this.router.navigateByUrl(this.returnUrl);
  }
})

}
}
