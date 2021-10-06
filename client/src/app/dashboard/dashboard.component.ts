import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IGroup, IGroups } from '../shared/models/dashboard-models/group';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  baseUrl = 'http://localhost:64156/api';
  groups: IGroups[];

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
    this.http.get<IGroups[]>(this.baseUrl + '/group')
    .subscribe((response) => {
      this.groups = response;
      console.log(this.groups);
    }, error => {
      console.log(error);
    });
  }

}
