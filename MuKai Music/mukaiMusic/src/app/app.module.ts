import { BrowserModule, HAMMER_GESTURE_CONFIG } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { AppComponent } from './app.component';
import { PlayerComponent } from './components/player/player.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { MatSliderModule } from '@angular/material/slider';
import { GestureConfig } from '@angular/material/core';
import { MkImgComponent } from './components/mk-img/mk-img.component';
import { MatGridListModule } from '@angular/material/grid-list';
import { PaletteComponent } from './components/palette/palette.component';
import { AppRoutingModule } from './app-routing.module';
import { PlaylistComponent } from './components/playlist/playlist.component';
import { ControlComponent } from './components/control/control.component';
import { MusicInfoComponent } from './components/music-info/music-info.component';
import { AccountInterceptor } from './services/network/accountInterceptor';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { SnackBarComponent } from './components/snackBar/snackBar.component';




@NgModule({
  declarations: [
    AppComponent,
    PlayerComponent,
    MkImgComponent,
    PlaylistComponent,
    PaletteComponent,
    ControlComponent,
    MusicInfoComponent,
    SnackBarComponent
  ],
  imports: [
    MatIconModule,
    MatSlideToggleModule,
    BrowserModule,
    MatSnackBarModule,
    AppRoutingModule,
    HttpClientModule,
    MatSliderModule,
    MatGridListModule,
    BrowserAnimationsModule
  ],
  providers: [
    { provide: HAMMER_GESTURE_CONFIG, useClass: GestureConfig },
    { provide: HTTP_INTERCEPTORS, useClass: AccountInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
