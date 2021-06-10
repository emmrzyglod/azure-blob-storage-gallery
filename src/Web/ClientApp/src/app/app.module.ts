import {BrowserModule} from '@angular/platform-browser';
import {NgModule} from '@angular/core';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientModule} from '@angular/common/http';
import {RouterModule} from '@angular/router';

import {AppComponent} from './app.component';
import {NavMenuComponent} from './nav-menu/nav-menu.component';
import {HomeComponent} from './home/home.component';
import {AddImageComponent} from './add-image/add-image.component';
import {ImagesListComponent} from './images-list/images-list.component';
import {ShopClient} from "./app-api";
import {AddProductComponent} from "./products/add-product/add-product.component";
import {ProductsListComponent} from "./products/products-list/products-list.component";
import {LoginUserComponent} from "./login-user/login-user.component";
import {AuthService} from "./services/auth.service";

@NgModule({
  declarations: [
    AppComponent,
    NavMenuComponent,
    HomeComponent,
    AddImageComponent,
    AddProductComponent,
    ImagesListComponent,
    ProductsListComponent,
    LoginUserComponent
  ],
  imports: [
    BrowserModule.withServerTransition({appId: 'ng-cli-universal'}),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      {path: '', component: HomeComponent, pathMatch: 'full'},
      {path: 'add-image', component: AddImageComponent},
      {path: 'add-product', component: AddProductComponent},
      {path: 'auth', component: LoginUserComponent},
    ]),
    ReactiveFormsModule
  ],
  providers: [ShopClient, AuthService],
  bootstrap: [AppComponent]
})
export class AppModule { }
