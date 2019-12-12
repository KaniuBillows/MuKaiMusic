import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContentComponent } from '../../components/content/content.component';
import { PlaylistComponent } from '../../components/playlist/playlist.component';
import { Routes, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { ExploreComponent } from '../../components/explore/explore.component';

const routes: Routes = [
  {
    path: '',
    component: ContentComponent,
    children: [
      {
        path: 'playlist',
        component: PlaylistComponent
      },
      {
        path: 'explore',
        loadChildren: () =>
          import('../explore/explore.module').then(m => m.ExploreModule)
      }
    ]
  },

];

@NgModule({
  declarations: [
    ContentComponent,
    PlaylistComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatSidenavModule,
    RouterModule.forChild(routes)
  ]
})
export class ContentModule { }
