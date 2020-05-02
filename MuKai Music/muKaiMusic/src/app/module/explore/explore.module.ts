import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ExploreComponent } from 'src/app/components/explore/explore.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BannerComponent } from 'src/app/components/banner/banner.component';
import { BoutiqueComponent } from 'src/app/components/boutique/boutique.component';
import { SearchbarComponent } from 'src/app/components/searchbar/searchbar.component';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { SearchResultComponent } from 'src/app/components/search-result/search-result.component';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
const routes: Routes = [
  {
    path: '',
    component: ExploreComponent,
    children: [{
      path: 'boutique',
      component: BoutiqueComponent
    }, {
      path: 'search/:key',
      component: SearchResultComponent
    }]
  }
]

@NgModule({
  declarations: [
    ExploreComponent,
    SearchbarComponent,
    SearchResultComponent,
    BannerComponent,
    BoutiqueComponent
  ],
  imports: [
    FormsModule,
    MatSnackBarModule,
    MatProgressSpinnerModule,
    MatToolbarModule,
    MatIconModule,
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class ExploreModule { }
