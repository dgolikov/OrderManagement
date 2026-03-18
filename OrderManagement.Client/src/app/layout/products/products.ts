import { Component, inject, signal } from '@angular/core';
import { ProductService } from '../../services/products.service';
import { IProduct } from '../../models/responses/product.model';
import { IPage } from '../../models/responses/page.model';

@Component({
  selector: 'app-products',
  imports: [],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsComponent {
  private readonly _productService: ProductService = inject(ProductService);
  public products = signal<IProduct[]>([]);

  constructor() {
    this._productService.getPage().subscribe((page: IPage<IProduct>) => {
      this.products.set(page.items);
    });
  }
}
