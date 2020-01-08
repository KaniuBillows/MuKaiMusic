import { Component, OnInit, Input, ViewChild, ElementRef } from '@angular/core';
import { TouchSequence } from 'selenium-webdriver';

@Component({
  selector: 'mk-img',
  templateUrl: './mk-img.component.html',
  styleUrls: ['./mk-img.component.scss']
})
export class MkImgComponent implements OnInit {
  @ViewChild('img', { static: true })
  private img: ElementRef;

  @ViewChild('loading', { static: true })
  private loading: ElementRef;

  private _src: string = "";

  private _loadSrc: string = "../../../assets/img/logo.png";

  private _complete: boolean = false;

  @Input()
  public get src() {
    return this._src + "?param=240y240";
  }
  public set src(value: string) {
    this._src = value;
  }

  @Input()
  public get loadSrc() {
    return this._loadSrc;
  }
  public set loadSrc(value: string) {
    this._loadSrc = value;
  }

  public get complete() {
    return this._complete;
  }


  constructor() {
  }

  ngOnInit() {
    let img = this.img.nativeElement as HTMLImageElement;

    img.onerror = ev => {
      this.img.nativeElement.style.opacity = '0';
      this.loading.nativeElement.style.opacity = '1';
    }

    img.onload = ev => {
      this.img.nativeElement.style.opacity = '1';
      this.loading.nativeElement.style.opacity = '0';
    }

  }


}
