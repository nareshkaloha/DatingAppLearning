import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

  vals: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getAllValues();
  }

  getAllValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(resp => { this.vals = resp; }, err => { console.log(err); });
  }
}
