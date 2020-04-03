import { Component, OnInit, Inject, EventEmitter } from '@angular/core';
import { ThemeService } from 'src/app/services/theme/theme.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AccountService } from 'src/app/services/network/account/account.service';
import { UserInfo } from 'src/app/entity/user';
import { ErrorStateMatcher } from '@angular/material/core';
import { FormControl, FormGroupDirective, NgForm } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  /**
   * 界面绑定model
   */
  public user: {
    username: string,
    password: string
  } = { password: "", username: "" }

  constructor(public theme: ThemeService,
    public dialogRef: MatDialogRef<LoginComponent>,
    private accountService: AccountService
  ) { }

  ngOnInit() {
  }
  public get currentTheme() {
    return this.theme.getThemeClass();
  }

  public errorInfo: string = "";

  public isLoading: boolean = false;

  public login() {
    if (this.user.password == "" || this.user.username == "") {
      return this.errorInfo = "请输入您的信息";
    }
    this.isLoading = true;
    this.accountService.login(this.user.username, this.user.password).subscribe(async res => {
      if (res.code != 200) {
        this.isLoading = false;
        this.errorInfo = res.error;
      } else {
        await this.accountService.getUserInfo().toPromise().then(() => {
          this.dialogRef.close();
          this.isLoading = false;
        });
      }
    });
  }

  public onInputChange() {
    this.errorInfo = "";
  }



}
