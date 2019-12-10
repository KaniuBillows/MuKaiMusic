import { Component } from '@angular/core';
import { ThemeService } from './services/theme/theme.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'Mukai Music';

  constructor(private theme: ThemeService) {

  }
  public get themeClass(): string {
    return this.theme.getThemeClass();
  }
}
