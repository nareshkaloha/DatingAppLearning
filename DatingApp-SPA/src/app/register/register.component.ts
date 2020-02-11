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

  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(resp=> {
      console.log('user registered');
    }, err => {
      console.log(err);
    });
    
    // console.log(this.model);

  }

  cancel() {
    console.log('cancelled .. ');
    this.canceled.emit(false);
  }
}
