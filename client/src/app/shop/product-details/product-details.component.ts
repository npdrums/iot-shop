import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { BasketService } from 'src/app/basket/basket.service';
import { IProduct } from 'src/app/shared/models/product';
import { ShopService } from '../shop.service';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  product: IProduct;
  quantity = 1;

  constructor(private shopService: ShopService,
              private activatedRoot: ActivatedRoute,
              private basketService: BasketService) { }

  ngOnInit(): void {
    this.loadProduct();
  }

  // tslint:disable-next-line: typedef
  addItemToBasket() {
    this.basketService.addItemToBasket(this.product, this.quantity);
  }

  // tslint:disable-next-line: typedef
  increaseQuantity() {
    this.quantity++;
  }

  // tslint:disable-next-line: typedef
  decreaseQuantity() {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  // tslint:disable-next-line: typedef
  loadProduct() {
    this.shopService.getProduct(+this.activatedRoot.snapshot.paramMap.get('id')).subscribe(product => {
      this.product = product;
    }, error => {
      console.log(error);
    });
  }
}
