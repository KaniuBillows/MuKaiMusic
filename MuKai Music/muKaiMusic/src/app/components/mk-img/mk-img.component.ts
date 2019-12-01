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
    return this._src;
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
    window.setInterval(() => {
      if (this.img.nativeElement.complete) {
        this.img.nativeElement.style.opacity = '1';
        this.loading.nativeElement.style.opacity = '0';
      } else {
        this.img.nativeElement.style.opacity = '0';
        this.loading.nativeElement.style.opacity = '1';
      }
    }, 50);
  }

  ngOnInit() {
  }


}
