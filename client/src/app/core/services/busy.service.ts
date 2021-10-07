import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root',
})
export class BusyService {
  busyRequestCount = 0;

  constructor(private ngxSpinnerService: NgxSpinnerService) {}

  // tslint:disable-next-line: typedef
  busy() {
    this.busyRequestCount++;
    this.ngxSpinnerService.show(undefined, {
      type: 'pacman',
      bdColor: 'rgba(255,255,255,0.8)',
      color: '#333333',
    });
  }

  // tslint:disable-next-line: typedef
  idle() {
    this.busyRequestCount--;
    if (this.busyRequestCount <= 0) {
      this.busyRequestCount = 0;
      this.ngxSpinnerService.hide();
    }
  }
}
