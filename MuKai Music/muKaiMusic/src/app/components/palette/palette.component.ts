import { Component, OnInit, Output, Input, ViewChild, ElementRef } from '@angular/core';
import { ThemeService } from 'src/app/services/theme/theme.service';

@Component({
  selector: 'app-palette',
  templateUrl: './palette.component.html',
  styleUrls: ['./palette.component.scss']
})
export class PaletteComponent implements OnInit {

  private _showPalette: boolean = false;

  constructor(public theme: ThemeService) {
  }
  ngOnInit() {
    document.addEventListener("click", (e) => {
      let palette = document.getElementById("palette-container");
      //palette 为null则说明一定未展开调色板
      //点击调色板区域内部
      if (palette == null || palette.contains(e.target as HTMLElement)) {
        return;
      } else {
        let skin = document.getElementById("skin");
        //如果点击区域不属于目标区域
        if (!skin.contains(e.target as HTMLElement)) {
          this._showPalette = !this._showPalette;
        }
      }
    });

  }
  public get showPalette(): boolean {
    return this._showPalette;
  }

  public set showPalette(value: boolean) {
    this._showPalette = value;
  }


  public setTheme(theme: string) {
    this.theme.setThemeClass(theme);
  }

  public chooseSkin() {
    this._showPalette = !this._showPalette;
  }

  public get Themes() {
    return this.theme.themes;
  }

}
