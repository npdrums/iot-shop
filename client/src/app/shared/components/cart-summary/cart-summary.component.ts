import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { CartService } from 'src/app/cart/cart.service';
import { Observable } from 'rxjs';
import { ICart, ICartItem } from '../../models/cart';

@Component({
  selector: 'app-cart-summary',
  templateUrl: './cart-summary.component.html',
  styleUrls: ['./cart-summary.component.scss']
})
export class CartSummaryComponent implements OnInit {
  cart$: Observable<ICart>;
  @Output() decrement: EventEmitter<ICartItem> = new EventEmitter<ICartItem>();
  @Output() increment: EventEmitter<ICartItem> = new EventEmitter<ICartItem>();
  @Output() remove: EventEmitter<ICartItem> = new EventEmitter<ICartItem>();
  @Input() isCart = true;

  constructor(private cartService: CartService) { }

  // tslint:disable-next-line: typedef
  ngOnInit() {
    this.cart$ = this.cartService.cart$;
  }

  // tslint:disable-next-line: typedef
  decrementItemQuantity(item: ICartItem) {
    this.decrement.emit(item);
  }

  // tslint:disable-next-line: typedef
  incrementItemQuantity(item: ICartItem) {
    this.increment.emit(item);
  }

  // tslint:disable-next-line: typedef
  removeCartItem(item: ICartItem) {
    this.remove.emit(item);
  }

}