import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Cart, ICart, ICartItem, ICartTotals } from '../shared/models/cart';
import { IProduct } from '../shared/models/product';
import { IDeliveryMethod } from '../shared/models/deliveryMethod';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  baseUrl = environment.apiUrl;
  private cartSource = new BehaviorSubject<ICart>(null);
  cart$ = this.cartSource.asObservable();
  private cartTotalSource = new BehaviorSubject<ICartTotals>(null);
  cartTotal$ = this.cartTotalSource.asObservable();
  shipping = 0;

  constructor(private http: HttpClient) {}

  // tslint:disable-next-line: typedef
  setShippingPrice(deliveryMethod: IDeliveryMethod) {
    this.shipping = deliveryMethod.price;
    const cart = this.getCurrentCartValue();
    cart.deliveryMethodId = deliveryMethod.id;
    cart.shippingPrice = deliveryMethod.price;
    this.calculateTotals();
    this.setCart(cart);
  }

  // tslint:disable-next-line: typedef
  createPaymentIntent() {
    return this.http
      .post(this.baseUrl + 'payment/' + this.getCurrentCartValue().id, {})
      .pipe(
        map((cart: ICart) => {
          this.cartSource.next(cart);
        })
      );
  }

  // tslint:disable-next-line: typedef
  getCart(id: string) {
    return this.http.get(this.baseUrl + 'shoppingcart?id=' + id).pipe(
      map((cart: ICart) => {
        console.log(cart);
        this.cartSource.next(cart);
        this.shipping = cart.shippingPrice;
        this.calculateTotals();
      })
    );
  }

  // tslint:disable-next-line: typedef
  setCart(cart: ICart) {
    console.log(cart);
    this.http.post(this.baseUrl + 'shoppingcart', cart).subscribe(
      (response: ICart) => {
        this.cartSource.next(response);
        this.calculateTotals();
        console.log(response);
      },
      (error) => {
        console.log(error);
      }
    );
  }

  // tslint:disable-next-line: typedef
  getCurrentCartValue() {
    return this.cartSource.value;
  }

  // tslint:disable-next-line: typedef
  addItemToCart(item: IProduct, quantity = 1) {
    const itemToAdd: ICartItem = this.mapProductItemToCartItem(item, quantity);
    console.log(itemToAdd);
    const cart = this.getCurrentCartValue() ?? this.createCart();
    cart.items = this.addOrUpdateItem(cart.items, itemToAdd, quantity);
    this.setCart(cart);
  }

  // tslint:disable-next-line: typedef
  increaseItemQuantity(item: ICartItem) {
    const cart = this.getCurrentCartValue();
    const foundItemIndex = cart.items.findIndex((x) => x.id === item.id);
    cart.items[foundItemIndex].quantity++;
    this.setCart(cart);
  }

  // tslint:disable-next-line: typedef
  decreaseItemQuantity(item: ICartItem) {
    const cart = this.getCurrentCartValue();
    const foundItemIndex = cart.items.findIndex((x) => x.id === item.id);
    if (cart.items[foundItemIndex].quantity > 1) {
      cart.items[foundItemIndex].quantity--;
      this.setCart(cart);
    } else {
      this.removeItemFromCart(item);
    }
  }

  // tslint:disable-next-line: typedef
  removeItemFromCart(item: ICartItem) {
    const cart = this.getCurrentCartValue();
    if (cart.items.some((x) => x.id === item.id)) {
      cart.items = cart.items.filter((i) => i.id !== item.id);
      if (cart.items.length > 0) {
        this.setCart(cart);
      } else {
        this.deleteCart(cart);
      }
    }
  }

  // tslint:disable-next-line: typedef
  deleteLocalCart(id: string) {
    this.cartSource.next(null);
    this.cartTotalSource.next(null);
    localStorage.removeItem('cart_id');
  }

  // tslint:disable-next-line: typedef
  deleteCart(cart: ICart) {
    return this.http
      .delete(this.baseUrl + 'shoppingcart?id=' + cart.id)
      .subscribe(
        () => {
          this.cartSource.next(null);
          this.cartTotalSource.next(null);
          localStorage.removeItem('cart_id');
        },
        (error) => {
          console.log(error);
        }
      );
  }

  // tslint:disable-next-line: typedef
  private calculateTotals() {
    const cart = this.getCurrentCartValue();
    const shipping = this.shipping;
    const subtotal = cart.items.reduce((a, b) => b.price * b.quantity + a, 0);
    const total = subtotal + shipping;
    this.cartTotalSource.next({ shipping, total, subtotal });
  }

  private addOrUpdateItem(
    items: ICartItem[],
    itemToAdd: ICartItem,
    quantity: number
  ): ICartItem[] {
    const index = items.findIndex((item) => item.id === itemToAdd.id);
    if (index === -1) {
      itemToAdd.quantity = quantity;
      items.push(itemToAdd);
    } else {
      items[index].quantity += quantity;
    }
    return items;
  }

  private createCart(): ICart {
    const cart = new Cart();
    localStorage.setItem('cart_id', cart.id); // browser storage
    return cart;
  }

  private mapProductItemToCartItem(
    item: IProduct,
    quantity: number
  ): ICartItem {
    return {
      id: item.id,
      productName: item.name,
      price: item.price,
      pictureUrl: item.pictureUrl,
      quantity,
      brand: item.productBrand,
      type: item.productType,
    };
  }
}
