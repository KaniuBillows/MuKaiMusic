import { Component, OnInit } from '@angular/core';
import { UserService } from 'src/app/services/network/user/user.service';
import { UserInfo } from 'src/app/entity/user';
import { MatDialog } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
  public userInfo: UserInfo;
  constructor(
    private userService: UserService,
    public dialog: MatDialog
  ) { }

  ngOnInit() {

  }
  public loginClick() {
    let dialogref = this.dialog.open(LoginComponent);
    dialogref.afterClosed().subscribe(result => {

    });
  }

  public registerClick() {

  }
}
