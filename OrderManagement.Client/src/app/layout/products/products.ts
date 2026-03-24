import { Component, inject, signal, computed } from '@angular/core';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { ProductService } from '../../services/products.service';
import { IProduct } from '../../models/responses/product.model';
import { IPage } from '../../models/responses/page.model';
import { InfiniteScrollDirective } from '../../directives/infinite-scroll.directive';

@Component({
  selector: 'app-products',
  imports: [MatCard, MatCardContent, MatProgressSpinner, InfiniteScrollDirective],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsComponent {
  private readonly _productService = inject(ProductService);

  private readonly _products = signal<IProduct[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _pageNumber = signal<number>(1);
  private readonly _hasMore = signal<boolean>(true);

  public readonly products = this._products.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();

  public readonly showNoMore = computed(() => !this._hasMore() && this._products().length > 0);

  private readonly _pageSize = 50;

  constructor() {
    this.loadProducts();
  }

  public onLoadMore(): void {
    if (this._isLoading() || !this._hasMore()) {
      return;
    }

    this.loadProducts();
  }

  private loadProducts(): void {
    this._isLoading.set(true);

    this._productService.getPage(this._pageNumber(), this._pageSize).subscribe({
      next: (page: IPage<IProduct>) => {
        this._products.update((current) => [...current, ...page.items]);
        this._hasMore.set(
          page.items.length === this._pageSize && this._pageNumber() * this._pageSize < page.total,
        );
        this._pageNumber.update((n) => n + 1);
        this._isLoading.set(false);
      },
      error: () => {
        this._isLoading.set(false);
      },
    });
  }
}
