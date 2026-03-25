import { Component, inject, signal, OnDestroy } from '@angular/core';
import { MatCard, MatCardContent, MatCardTitle } from '@angular/material/card';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { Subject, takeUntil } from 'rxjs';
import { OrdersService } from '../../services/orders.service';
import { IOrder } from '../../models/responses/order.model';
import { IPage } from '../../models/responses/page.model';
import { InfiniteScrollDirective } from '../../directives/infinite-scroll.directive';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [
    MatCard,
    MatCardContent,
    MatCardTitle,
    MatProgressSpinner,
    InfiniteScrollDirective,
  ],
  templateUrl: './orders.html',
  styleUrl: './orders.scss',
})
export class OrdersComponent implements OnDestroy {
  private readonly _ordersService = inject(OrdersService);

  private readonly _orders = signal<IOrder[]>([]);
  private readonly _isLoading = signal<boolean>(false);
  private readonly _pageNumber = signal<number>(1);
  private readonly _hasMore = signal<boolean>(true);

  private readonly _destroy$ = new Subject<void>();

  public readonly orders = this._orders.asReadonly();
  public readonly isLoading = this._isLoading.asReadonly();
  public readonly hasMore = this._hasMore.asReadonly();

  private readonly _pageSize = 20;

  constructor() {
    this.loadOrders();
  }

  public ngOnDestroy(): void {
    this._destroy$.next();
    this._destroy$.complete();
  }

  public onLoadMore(): void {
    if (this._isLoading() || !this._hasMore()) {
      return;
    }

    this.loadOrders();
  }

  private loadOrders(): void {
    this._isLoading.set(true);

    this._ordersService.getPage(this._pageNumber(), this._pageSize)
      .pipe(takeUntil(this._destroy$))
      .subscribe({
        next: (page: IPage<IOrder>) => {
          this._orders.update((current) => [...current, ...page.items]);
          this._hasMore.set(
            page.items.length === this._pageSize &&
            this._pageNumber() * this._pageSize < page.total,
          );
          this._pageNumber.update((n) => n + 1);
          this._isLoading.set(false);
        },
        error: () => {
          this._isLoading.set(false);
        },
      });
  }

  public calculateLineItemTotal(quantity: number, price: number): number {
    return quantity * price;
  }

  public formatPrice(amount: number): string {
    return `${this.formatNumber(amount)} ₽`;
  }

  private formatNumber(value: number): string {
    return value.toLocaleString('ru-RU', {
      minimumFractionDigits: 0,
      maximumFractionDigits: 2,
    });
  }

  public translateStatus(status: string): string {
    const statusMap: Record<string, string> = {
      'Created': 'Создан',
      'Approved': 'Подтверждён',
      'InShipping': 'В доставке',
      'Shipped': 'Доставлен',
    };
    return statusMap[status] ?? status;
  }
}
