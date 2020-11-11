import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { ICart, ICartItem } from '../shared/models/cart';
import { CartService } from './cart.service';


@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cart$: Observable<ICart>;

  constructor(private cartService: CartService) { }

  ngOnInit(): void {
    this.cart$ = this.cartService.cart$;
  }

  // tslint:disable-next-line: typedef
  removeCartItem(item: ICartItem) {
    this.cartService.removeItemFromCart(item);
  }

  // tslint:disable-next-line: typedef
  increaseItemQuantity(item: ICartItem) {
    this.cartService.increaseItemQuantity(item);
  }

  // tslint:disable-next-line: typedef
  decreaseItemQuantity(item: ICartItem) {
    this.cartService.decreaseItemQuantity(item);
  }

}
