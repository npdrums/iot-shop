import { Component, OnInit } from '@angular/core';
import { IOrder } from 'src/app/shared/models/order';
import { ActivatedRoute } from '@angular/router';
import { OrdersService } from '../orders.service';

@Component({
  selector: 'app-order-detailed',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent implements OnInit {
  order: IOrder;

  constructor(
    private route: ActivatedRoute,
    private ordersService: OrdersService
  ) {}

  // tslint:disable-next-line: typedef
  ngOnInit() {
    this.ordersService
      .getOrderDetails(+this.route.snapshot.paramMap.get('id'))
      .subscribe(
        (order: IOrder) => {
          this.order = order;
        },
        (error: any) => {
          console.log(error);
        }
      );
  }
}
