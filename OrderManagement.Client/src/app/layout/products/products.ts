import { Component, inject, signal, computed, OnDestroy } from '@angular/core';
import { MatCard, MatCardContent } from '@angular/material/card';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, takeUntil } from 'rxjs';
import { ProductService } from '../../services/products.service';
import { IProduct } from '../../models/responses/product.model';
import { IPage } from '../../models/responses/page.model';
import { InfiniteScrollDirective } from '../../directives/infinite-scroll.directive';

@Component({
  selector: 'app-products',
  imports: [
    MatCard,
    MatCardContent,
    MatProgressSpinner,
    InfiniteScrollDirective,
    MatFormFieldModule,
    MatInputModule,
    FormsModule,
    MatFormFieldModule,
  ],
  templateUrl: './products.html',
  styleUrl: './products.scss',
})
export class ProductsComponent implements OnDestroy {
  private readonly _productService = inject(ProductService);

  private readonly _products = signal<IProduct[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _pageNumber = signal<number>(1);
  private readonly _hasMore = signal<boolean>(true);

  private readonly _searchTerm = signal<string>('');
  private readonly _searchInput = signal<string>('');
  private readonly _searchSubject = new Subject<string>();
  private readonly _destroy$ = new Subject<void>();

  public readonly products = this._products.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly searchInput = this._searchInput.asReadonly();

  public readonly nothingFound = computed(() => this._searchTerm() && this._products().length == 0);

  private readonly _pageSize = 50;

  constructor() {
    this._searchSubject.pipe(debounceTime(300), takeUntil(this._destroy$)).subscribe((term) => {
      if (term !== this._searchTerm()) {
        this._searchTerm.set(term);
        this._products.set([]);
        this._pageNumber.set(1);
        this._hasMore.set(true);
        this.loadProducts();
      }
    });

    this.loadProducts();
  }

  public onSearchChange(value: string): void {
    this._searchInput.set(value);
    this._searchSubject.next(value);
  }

  public ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
    this._searchSubject.complete();
  }

  public onLoadMore(): void {
    if (this._isLoading() || !this._hasMore()) {
      return;
    }

    this.loadProducts();
  }

  private loadProducts(): void {
    this._isLoading.set(true);
    const searchTerm = this._searchTerm() || undefined;

    this._productService.getPage(this._pageNumber(), this._pageSize, searchTerm).subscribe({
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
