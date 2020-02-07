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
    this.accountService.loginSuccess.subscribe(() => {
      this.dialogRef.close();
    });
    this.accountService.loginFailed.subscribe((err: string) => {
      this.errorInfo = err;
    })
  }
  public get currentTheme() {
    return this.theme.getThemeClass();
  }

  public errorInfo: string = "";

  public login() {
    this.accountService.logIn(this.user.username, this.user.password);
  }

  public onInputChange(e) {
    this.errorInfo = "";
  }



}
