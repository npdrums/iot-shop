import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ICartTotals } from '../../models/cart';
import { CartService } from 'src/app/cart/cart.service';

@Component({
  selector: 'app-order-totals',
  templateUrl: './order-totals.component.html',
  styleUrls: ['./order-totals.component.scss']
})
export class OrderTotalsComponent implements OnInit {
  cartTotal$: Observable<ICartTotals>;

  constructor(private cartService: CartService) { }

  // tslint:disable-next-line: typedef
  ngOnInit() {
    this.cartTotal$ = this.cartService.cartTotal$;
  }

}
