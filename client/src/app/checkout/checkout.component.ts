import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { AccountService } from '../account/account.service';
import { CartService } from '../cart/cart.service';
import { ICartTotals } from '../shared/models/cart';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss'],
})
export class CheckoutComponent implements OnInit {
  checkoutForm: FormGroup;
  cartTotals$: Observable<ICartTotals>;

  constructor(
    private formBuilder: FormBuilder,
    private accountService: AccountService,
    private cartService: CartService
  ) {}

  ngOnInit(): void {
    this.createCheckoutForm();
    this.getAddressFormValues();
    this.getDeliveryMethodValue();
    this.cartTotals$ = this.cartService.cartTotal$;
  }

  // tslint:disable-next-line: typedef
  createCheckoutForm() {
    this.checkoutForm = this.formBuilder.group({
      addressForm: this.formBuilder.group({
        firstName: [null, Validators.required],
        lastName: [null, Validators.required],
        street: [null, Validators.required],
        city: [null, Validators.required],
        state: [null, Validators.required],
        zipcode: [null, Validators.required],
      }),
      deliveryForm: this.formBuilder.group({
        deliveryMethod: [null, Validators.required],
      }),
      paymentForm: this.formBuilder.group({
        nameOnCard: [null, Validators.required],
      }),
    });
  }

  // tslint:disable-next-line: typedef
  getAddressFormValues() {
    this.accountService.getUserAddress().subscribe(
      (address) => {
        if (address) {
          this.checkoutForm.get('addressForm').patchValue(address);
        }
      },
      (error) => {
        console.log(error);
      }
    );
  }

  // tslint:disable-next-line: typedef
  getDeliveryMethodValue() {
    const cart = this.cartService.getCurrentCartValue();
    if (cart.deliveryMethodId !== null) {
      this.checkoutForm
        .get('deliveryForm')
        .get('deliveryMethod')
        .patchValue(cart.deliveryMethodId.toString());
    }
  }
}
