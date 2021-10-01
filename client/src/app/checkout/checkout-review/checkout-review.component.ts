import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CartService } from 'src/app/cart/cart.service';
import { ICart } from 'src/app/shared/models/cart';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
  cart$: Observable<ICart>;

  constructor(private cartService: CartService) { }

  // tslint:disable-next-line: typedef
  ngOnInit() {
    this.cart$ = this.cartService.cart$;
  }

}