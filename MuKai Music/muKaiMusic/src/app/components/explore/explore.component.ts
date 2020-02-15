import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-explore',
  templateUrl: './explore.component.html',
  styleUrls: ['./explore.component.scss']
})
export class ExploreComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit() {
  }

  /**
   * 订阅searchbar组件的输入完毕事件，开始进行搜索
   * @param value
   */
  public search(value: string) {
    this.router.navigate(['/content/explore/search', value]);
  }
}
