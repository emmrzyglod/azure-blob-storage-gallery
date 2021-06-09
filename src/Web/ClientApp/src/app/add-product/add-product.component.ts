import {Component, Inject} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Subject} from 'rxjs';

@Component({
  selector: 'app-add-product',
  templateUrl: './add-product.component.html'
})
export class AddProductComponent {

  baseUrl: string;

  form: FormGroup;
  uploadInProgress = false;

  imageAdded: Subject<void> = new Subject<void>();

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private fb: FormBuilder) {
    this.baseUrl = baseUrl;
    this.form = this.fb.group({
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

    if(file != null) {

      let formData:FormData = new FormData();
      formData.append('file', file, file.name);
      let headers = new HttpHeaders();

      headers.append('Content-Type', 'multipart/form-data');
      headers.append('Accept', 'application/json');

      this.uploadInProgress = true;

      this.http
        .post<string>(this.baseUrl + 'Gallery/AddPhoto', formData, {reportProgress: true, observe: 'events', headers: headers})
        .subscribe(result => {
          console.log(result);
          this.imageAdded.next();
        }, error => console.error(error));
    }
  }
}
