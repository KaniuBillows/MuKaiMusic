import { Component, OnInit } from '@angular/core';
import { ThemeService } from 'src/app/services/theme/theme.service';

@Component({
  selector: 'app-content',
  templateUrl: './content.component.html',
  styleUrls: ['./content.component.scss']
})
export class ContentComponent implements OnInit {

  constructor(
    public theme: ThemeService
  ) { }

  ngOnInit() {
  }
  public get themeClass() {
    return this.theme.getThemeClass();
  }

  public class(value: string): string {
    return location.href.includes(value) ? this.themeClass : '';
  }

  public current(value: string): string {
    return location.href.includes(value) ? '' : 'normal';
  }
}
