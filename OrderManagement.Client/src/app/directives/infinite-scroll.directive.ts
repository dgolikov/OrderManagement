import {
  Directive,
  ElementRef,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';

@Directive({
  selector: '[appInfiniteScroll]',
  standalone: true,
})
export class InfiniteScrollDirective implements OnInit, OnDestroy {
  private _observer: IntersectionObserver | null = null;

  @Output() public reachedBottom = new EventEmitter<void>();

  constructor(private readonly _element: ElementRef) {}

  public ngOnInit(): void {
    this._observer = new IntersectionObserver(
      (entries) => {
        if (entries[0].isIntersecting) {
          this.reachedBottom.emit();
        }
      },
      {
        root: null,
        rootMargin: '100px',
        threshold: 0,
      }
    );

    this._observer.observe(this._element.nativeElement);
  }

  public ngOnDestroy(): void {
    if (this._observer) {
      this._observer.disconnect();
      this._observer = null;
    }
  }
}
