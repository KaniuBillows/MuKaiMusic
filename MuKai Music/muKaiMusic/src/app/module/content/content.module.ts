import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ContentComponent } from '../../components/content/content.component';
import { PlaylistComponent } from '../../components/playlist/playlist.component';
import { Routes, RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatIconModule } from '@angular/material/icon';
import { UserComponent } from 'src/app/components/user/user.component';
import { MatExpansionModule } from '@angular/material/expansion';

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
    PlaylistComponent,
    UserComponent
  ],
  imports: [
    CommonModule,
    MatIconModule,
    MatExpansionModule,
      MatSidenavModule,
    RouterModule.forChild(routes)
  ]
})
export class ContentModule { }
