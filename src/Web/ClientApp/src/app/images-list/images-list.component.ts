import {Component, Inject, Input, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {FormBuilder} from '@angular/forms';
import {finalize} from 'rxjs/operators';
import { saveAs } from 'file-saver';
import {Observable, Subscription} from 'rxjs';

interface ImagesList {
   baseUrl: string;
   imagesPaths: string[];
}

@Component({
  selector: 'app-images-list',
  templateUrl: './images-list.component.html'
})
export class ImagesListComponent implements OnInit {

  @Input() events: Observable<void>;

  private eventsSubscription: Subscription;

  baseUrl: string;

  listRequestInProgress = false;
  deleteRequestInProgress = false;
  getSingleRequestInProgress = false;

  baseImageUrl: string;
  imagesPaths: string[];

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string, private fb: FormBuilder) {
    this.baseUrl = baseUrl;
  }

  ngOnInit() {
    this.eventsSubscription = this.events.subscribe(() => this.getImages());
    this.getImages();
  }

  getImages(): void {

      this.listRequestInProgress = true;

      this.http
        .get<ImagesList>(this.baseUrl + 'Gallery/PhotosList')
        .pipe(finalize(() => {
          this.listRequestInProgress = true;
        }))
        .subscribe(result => {
          this.baseImageUrl = result.baseUrl;
          this.imagesPaths = result.imagesPaths;
        }, error => console.error(error));
  }

  deleteImage(path: string): void {
    this.deleteRequestInProgress = true;
    this.http
      .delete(this.baseUrl + 'Gallery/DeletePhoto', {params: { photoName: path }})
      .pipe(finalize(() => {
        this.deleteRequestInProgress = false;
      }))
      .subscribe(
        () => { this.removePath(path); },
          error => console.error(error)
      );
  }

  private removePath(path: string): void {
    const index: number = this.imagesPaths.indexOf(path, 0);
    this.imagesPaths.splice(index, 1);
  }

  downloadFile(name: string): void {
    this.getSingleRequestInProgress = true;
    this.http
      .get<any>(this.baseUrl + 'Gallery/Get', { responseType: 'blob' as 'json', params: {
          photoName: name
        }})
      .pipe(finalize(() => {
        this.getSingleRequestInProgress = false;
      }))
      .subscribe(result => {
        const data = new Blob([result]);
        saveAs(data, name);
      }, error => console.error(error));
  }

  ngOnDestroy() {
    this.eventsSubscription.unsubscribe();
  }

}
