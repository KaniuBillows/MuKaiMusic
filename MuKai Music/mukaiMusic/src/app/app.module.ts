import { BrowserModule,HAMMER_GESTURE_CONFIG} from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { PlayerComponent } from './components/player/player.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HttpClientModule } from '@angular/common/http';
import { MatSliderModule } from '@angular/material/slider';
import { GestureConfig } from '@angular/material/core';
import { MkImgComponent } from './components/mk-img/mk-img.component';
import { GithubComponent } from './components/github/github.component';

@NgModule({
  declarations: [
    AppComponent,
    PlayerComponent,
    MkImgComponent,
    GithubComponent
  ],
  imports: [
    MatIconModule,
    MatSlideToggleModule,
    BrowserModule,
    HttpClientModule,
    MatSliderModule,
    AppRoutingModule,
    BrowserAnimationsModule
  ],
  providers: [
    {provide:HAMMER_GESTURE_CONFIG,useClass:GestureConfig}
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
