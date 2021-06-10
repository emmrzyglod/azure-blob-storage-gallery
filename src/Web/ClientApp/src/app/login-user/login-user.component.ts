import {Component, Inject} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Subject} from 'rxjs';
import {AuthService} from "../services/auth.service";
import {Route, Router} from "@angular/router";

@Component({
  selector: 'app-login-user',
  templateUrl: './login-user.component.html'
})
export class LoginUserComponent {

  baseUrl: string;

  form: FormGroup;

  imageAdded: Subject<void> = new Subject<void>();

  constructor(
    private authService: AuthService,
    private http: HttpClient,
    private router: Router,
    @Inject('BASE_URL') baseUrl: string,
    private fb: FormBuilder)
  {
    this.baseUrl = baseUrl;
    this.form = this.fb.group({
      email: ['', Validators.required]
    });
  }

  sendForm(event) {
    this.authService.email = this.form.controls.email.value;
    return this.router.navigate(['/add-product']);
  }
}
