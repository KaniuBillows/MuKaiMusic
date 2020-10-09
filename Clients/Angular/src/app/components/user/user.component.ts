import { Component, OnInit } from '@angular/core';
import { UserInfo } from 'src/app/entity/user';
import { MatDialog } from '@angular/material/dialog';
import { LoginComponent } from '../login/login.component';
import { AccountService } from 'src/app/services/network/account/account.service';
import { RegisterComponent } from '../register/register.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {

  constructor(
    public dialog: MatDialog,
    public account: AccountService
  ) { }
  public showOption: boolean = false;

  ngOnInit() {
    document.addEventListener("click", (e) => {
      let options = document.getElementById("options-container");
      //为展开的情况
      if (options == null) return;
      //防止点击昵称时不做处理
      if (e.target == document.getElementById("nickName")) return;
      //点击除昵称之外的所有元素
      this.showOption = false;
    });
  }

  public nickNameClick() {
    this.showOption = !this.showOption;
  }

  public loginClick() {
    this.dialog.open(LoginComponent);
  }

  public async logoutClick() {
    this.showOption = false;
    await this.account.logOut();
  }

  public registerClick() {
    this.dialog.open(RegisterComponent);
  }
}
