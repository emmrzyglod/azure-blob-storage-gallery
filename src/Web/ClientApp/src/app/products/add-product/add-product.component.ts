import {Component, Inject} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Subject} from 'rxjs';
import {FileParameter, ShopClient} from "../../app-api";

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html'
})
export class AddProductComponent {

  baseUrl: string;

  form: FormGroup;
  uploadInProgress = false;

  imageAdded: Subject<void> = new Subject<void>();

  constructor(
    private shopClient: ShopClient,
    private http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    private fb: FormBuilder)
  {
    this.baseUrl = baseUrl;
    this.form = this.fb.group({
      name: ['', Validators.required],
      description: ['', Validators.required],
      price: [null, Validators.required],
      photo: ['', Validators.required]
    });
  }

  addImage(event) {
    if (event.target.files.length > 0) {
      const file = event.target.files[0];
      this.form.get('photo').setValue(file);
    }
  }

  sendForm(event) {

    let file: File = this.form.controls.photo.value;
    console.log(this.form);
    console.log(this.form.controls);
    console.log(this.form.controls.name);
    const name = this.form.controls.name.value;
    const description = this.form.controls.description.value;
    const price = this.form.controls.price.value;
    const fileToRequest: FileParameter = {
      data: file,
      fileName: file.name
    };

    this.shopClient
      .addProduct(name, description, price, fileToRequest)
      .subscribe();
  }
}
