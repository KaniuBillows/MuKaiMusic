import { Component, OnInit, Output, Input } from '@angular/core';
import { ThemeService } from 'src/app/services/theme/theme.service';

@Component({
  selector: 'app-palette',
  templateUrl: './palette.component.html',
  styleUrls: ['./palette.component.scss']
})
export class PaletteComponent implements OnInit {

  private _showPalette: boolean = false;
  constructor(public theme: ThemeService) {
    window.addEventListener('click', (ev: MouseEvent) => {
      let ele = ev.srcElement as HTMLElement
      if (ele.id == 'contain' || ele.id == 'colors' || ele.className == 'mat-figure') {
        return;
      }
      if (ele.id != 'skin' && ele.id != 'skin-icon') {
        if (this._showPalette) {
          this._showPalette = false;
        }
      }
    })
  }

  @Input()
  public get showPalette(): boolean {
    return this._showPalette;
  }

  public set showPalette(value: boolean) {
    this._showPalette = value;
  }

  ngOnInit() {

  }
  public setTheme(theme: string) {
    this.theme.setThemeClass(theme);
  }

  public chooseSkin() {
    this._showPalette = !this._showPalette;
  }


}
