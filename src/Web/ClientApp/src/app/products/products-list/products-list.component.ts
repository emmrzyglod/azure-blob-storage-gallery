import {Component, Inject, Input, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {finalize} from 'rxjs/operators';
import {Observable, Subscription} from 'rxjs';
import {ProductDto, ShopClient} from "../../app-api";
import {AuthService} from "../../services/auth.service";

@Component({
  selector: 'app-products-list',
  templateUrl: './products-list.component.html'
})
export class ProductsListComponent implements OnInit {

  @Input() events: Observable<void>;

  private eventsSubscription: Subscription;

  baseUrl: string;

  listRequestInProgress = false;
  deleteRequestInProgress = false;
  getSingleRequestInProgress = false;

  products?: ProductDto[]

  constructor(
    private shopClient: ShopClient,
    private authService: AuthService,
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string)
  {
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.eventsSubscription = this.events.subscribe(() => this.getProducts());
    this.getProducts();
    console.log(this.authService.email);
  }

  getProducts(): void {

      this.listRequestInProgress = true;

      this.shopClient
        .browseProducts()
        .pipe(finalize(() => {
          this.listRequestInProgress = true;
        }))
        .subscribe(result => {
          this.products = result.products;
        }, error => console.error(error));
  }

  ngOnDestroy() {
    this.eventsSubscription.unsubscribe();
  }

}
