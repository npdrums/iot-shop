import { CdkStepper } from '@angular/cdk/stepper';
import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CartService } from 'src/app/cart/cart.service';
import { ICart } from 'src/app/shared/models/cart';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-checkout-review',
  templateUrl: './checkout-review.component.html',
  styleUrls: ['./checkout-review.component.scss']
})
export class CheckoutReviewComponent implements OnInit {
  @Input() appStepper: CdkStepper;
  cart$: Observable<ICart>;

  constructor(private cartService: CartService) { }

  // tslint:disable-next-line: typedef
  ngOnInit() {
    this.cart$ = this.cartService.cart$;
  }

  // tslint:disable-next-line: typedef
  createPaymentIntent() {
    return this.cartService.createPaymentIntent().subscribe((response: any) => {
      this.appStepper.next();
    }, error => {
      console.log(error);
    });
  }

}
