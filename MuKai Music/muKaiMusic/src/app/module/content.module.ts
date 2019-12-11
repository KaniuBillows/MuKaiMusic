import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContentComponent } from '../components/content/content.component';
import { PlaylistComponent } from '../components/playlist/playlist.component';

import { Routes, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { ExploreComponent } from '../components/explore/explore.component';

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
        component: ExploreComponent
      }
    ]
  },
];

@NgModule({
  declarations: [
    ContentComponent,
    PlaylistComponent,
    ExploreComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatSidenavModule,
    RouterModule.forChild(routes)
  ]
})
export class ContentModule { }
