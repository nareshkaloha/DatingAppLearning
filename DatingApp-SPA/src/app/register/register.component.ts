import { AlertifyService } from './../_services/alertify.service';
import { AuthService } from './../_services/auth.service';
import { HttpClient } from '@angular/common/http';
import { Component, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  @Output('Canceled') canceled = new EventEmitter();

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(resp => {
      this.alertify.success('user registered');
    }, err => {
      this.alertify.error(err);
    });
    
    // console.log(this.model);

  }

  cancel() {
    console.log('canceled .. ');
    this.canceled.emit(false);
  }
}
