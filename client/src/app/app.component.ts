import { Component, OnInit } from '@angular/core';
import { CartService } from './cart/cart.service';
import { AccountService } from './account/account.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  title = 'IOT Shop';

  constructor(
    private cartService: CartService,
    private accountService: AccountService
  ) {}

  ngOnInit(): void {
    this.loadCart();
    this.loadCurrentUser();
  }

  // tslint:disable-next-line: typedef
  loadCurrentUser() {
    const token = localStorage.getItem('token');
    this.accountService.loadCurrentUser(token).subscribe(
      () => {
        console.log('user loaded');
      },
      (error) => {
        console.log(error);
      }
    );
  }

  // tslint:disable-next-line: typedef
  loadCart() {
    const cartId = localStorage.getItem('cart_id');
    if (cartId) {
      this.cartService.getCart(cartId).subscribe(
        () => {
          console.log('initialized cart');
        },
        (error) => {
          console.log(error);
        }
      );
    }
  }
}
