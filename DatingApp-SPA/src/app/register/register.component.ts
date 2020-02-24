import { Router } from '@angular/router';
import { User } from './../_models/user';
import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { BsDatepickerConfig } from 'ngx-bootstrap';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  user: User;
  @Output('Canceled') canceled = new EventEmitter();
  registerForm: FormGroup;
  bsConfig: Partial<BsDatepickerConfig>;

  constructor(
    private authService: AuthService,
    private alertify: AlertifyService,
    private router: Router,
    private fb: FormBuilder) { }

  ngOnInit() {
    // this.registerForm = new FormGroup( {
    //   username: new FormControl('', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4)]),
    //   confirmPassword: new FormControl('', [Validators.required])
    // }, this.passwordMatchValidator);

    //above one using form builder    
    this.bsConfig = {
      containerClass: 'theme-red'
    };
    this.createRegisterForm();
  }

  createRegisterForm() {
    this.registerForm = this.fb.group( {
      gender: ['male'],
      username: ['', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator}
    );
  }

  get username() {
    return this.registerForm.get('username');
  }

  get password() {
    return this.registerForm.get('password');
  }

  get confirmPassword() {
    return this.registerForm.get('confirmPassword');
  }

  get gender() {
    return this.registerForm.get('gender');
  }

  get dateOfBirth() {
    return this.registerForm.get('dateOfBirth');
  }

  get knownAs() {
    return this.registerForm.get('knownAs');
  }

  get city() {
    return this.registerForm.get('city');
  }

  get country() {
    return this.registerForm.get('country');
  }

  passwordMatchValidator(g: FormGroup) {
    return g.get('password').value === g.get('confirmPassword').value ? null : {passwordsMismatch: true };
  }

  register() {

    if (this.registerForm.valid) {
      this.user = Object.assign({}, this.registerForm.value);

      this.authService.register(this.user).subscribe(resp => {
        this.alertify.success('user registered');
    }, err => {
      this.alertify.error(err);
    }, () => {
      this.authService.login(this.user).subscribe( resp => {
        this.router.navigate(['/members']);
      });
    }
      );
    }

    // console.log(this.model);
    //console.log(this.registerForm.value);
  }

  cancel() {
    console.log('canceled .. ');
    this.canceled.emit(false);
  }
}
