import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Routes, RouterModule } from '@angular/router';
import { ExploreComponent } from 'src/app/components/explore/explore.component';

const routes: Routes = [
  {
    path: '',
    component: ExploreComponent
  }
]

@NgModule({
  declarations: [ExploreComponent],
  imports: [
    CommonModule,
    RouterModule.forChild(routes)
  ]
})
export class ExploreModule { }
