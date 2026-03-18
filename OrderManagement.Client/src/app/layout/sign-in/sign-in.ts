import { Component, computed, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { AuthService } from '../../services/auth.service';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-sign-in',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
  templateUrl: './sign-in.html',
  styleUrl: './sign-in.scss',
})
export class SignInComponent {
  private readonly _authService: AuthService = inject(AuthService);

  public signInForm = new FormGroup(
    {
      email: new FormControl<string>('', [Validators.required, Validators.email]),
      password: new FormControl<string>('', [Validators.required]),
    },
    { updateOn: 'blur' },
  );

  public get email(): FormControl<string> {
    return this.signInForm.get('email') as FormControl<string>;
  }

  public get password(): FormControl<string> {
    return this.signInForm.get('password') as FormControl<string>;
  }

  public onSingIn(): void {
    if (this.signInForm.invalid) {
      return;
    }

    this._authService.login({ email: this.email.value, password: this.password.value }).subscribe({
      error: (error) => console.error(error),
    });
  }
}
