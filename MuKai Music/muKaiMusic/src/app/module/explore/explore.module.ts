import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ExploreComponent } from 'src/app/components/explore/explore.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { BannerComponent } from 'src/app/components/banner/banner.component';
import { BoutiqueComponent } from 'src/app/components/boutique/boutique.component';
const routes: Routes = [
  {
    path: '',
    component: ExploreComponent,
    children: [{
      path: 'boutique',
      component: BoutiqueComponent
    }]
  }
]

@NgModule({
  declarations: [
    ExploreComponent,
    BannerComponent,
    BoutiqueComponent
  ],
  imports: [
    MatToolbarModule,
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class ExploreModule { }
