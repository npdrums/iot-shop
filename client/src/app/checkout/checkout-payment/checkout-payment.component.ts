import { Component, OnInit, Input, AfterViewInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { CartService } from 'src/app/cart/cart.service';
import { CheckoutService } from '../checkout.service';
import { ToastrService } from 'ngx-toastr';
import { ICart } from 'src/app/shared/models/cart';
import { IOrder } from 'src/app/shared/models/order';
import { Router, NavigationExtras } from '@angular/router';
import { environment } from 'src/environments/environment';

declare var Stripe;

@Component({
  selector: 'app-checkout-payment',
  templateUrl: './checkout-payment.component.html',
  styleUrls: ['./checkout-payment.component.scss']
})

export class CheckoutPaymentComponent implements AfterViewInit, OnDestroy {
  @Input() checkoutForm: FormGroup;
  @ViewChild('cardNumber', {static: true}) cardNumberElement: ElementRef;
  @ViewChild('cardExpiry', {static: true}) cardExpiryElement: ElementRef;
  @ViewChild('cardCVV', {static: true}) cardCVVElement: ElementRef;
  stripe: any;
  cardNumber: any;
  cardExpiry: any;
  cardCVV: any;
  cardErrors: any;
  cardHandler = this.onChange.bind(this);
  loading = false;
  cardNumberValid = false;
  cardExpiryValid = false;
  cardCvvValid = false;

  constructor(
    private cartService: CartService,
    private checkoutService: CheckoutService,
    private toastr: ToastrService,
    private router: Router) { }

  // tslint:disable-next-line: typedef
  ngOnDestroy() {
    this.cardNumber.destroy();
    this.cardExpiry.destroy();
    this.cardCVV.destroy();
    }

  // tslint:disable-next-line: typedef
  ngAfterViewInit() {
    this.stripe = Stripe(environment.stripePublicKey);
    const elements = this.stripe.elements();

    this.cardNumber = elements.create('cardNumber');
    this.cardNumber.mount(this.cardNumberElement.nativeElement);
    this.cardNumber.addEventListener('change', this.cardHandler);

    this.cardExpiry = elements.create('cardExpiry');
    this.cardExpiry.mount(this.cardExpiryElement.nativeElement);
    this.cardExpiry.addEventListener('change', this.cardHandler);

    this.cardCVV = elements.create('cardCVV');
    this.cardCVV.mount(this.cardCVVElement.nativeElement);
    this.cardCVV.addEventListener('change', this.cardHandler);
  }

  // tslint:disable-next-line: typedef
  onChange(event) {
    if (event.error) {
      this.cardErrors = event.error.message;
    } else {
      this.cardErrors = null;
    }

    switch (event.elementType) {
      case 'cardNumber':
        this.cardNumberValid = event.complete;
        break;
      case 'cardExpiry':
        this.cardExpiryValid = event.complete;
        break;
      case 'cardCvc':
        this.cardCvvValid = event.complete;
        break;
    }
  }

  // tslint:disable-next-line: typedef
  async submitOrder() {
    this.loading = true;
    const cart = this.cartService.getCurrentCartValue();
    try {
      const createdOrder = await this.createOrder(cart);
      const paymentResult = await this.confirmPaymentWithStripe(cart);

      if (paymentResult.paymentIntent) {
        this.cartService.deleteCart(cart);
        const navigationExtras: NavigationExtras = {state: createdOrder};
        this.router.navigate(['checkout/success'], navigationExtras);
      } else {
        this.toastr.error(paymentResult.error.message);
      }
      this.loading = false;
    } catch (error) {
      this.loading = false;
    }
  }

  // tslint:disable-next-line: typedef
  private async confirmPaymentWithStripe(basket) {
    return this.stripe.confirmCardPayment(basket.clientSecret, {
      payment_method: {
        card: this.cardNumber,
        billing_details: {
          name: this.checkoutForm.get('paymentForm').get('nameOnCard').value
        }
      }
    });
  }

  // tslint:disable-next-line: typedef
  private async createOrder(cart: ICart) {
    const orderToCreate = this.getOrderToCreate(cart);
    return this.checkoutService.creatOrder(orderToCreate).toPromise();
  }

  // tslint:disable-next-line: typedef
  private getOrderToCreate(cart: ICart) {
    return {
      cartId: cart.id,
      deliveryMethodId: +this.checkoutForm.get('deliveryForm')
        .get('deliveryMethod').value,
      shipToAddress: this.checkoutForm.get('addressForm').value
    };
  }

}
