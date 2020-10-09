import { Component, OnInit, Inject, EventEmitter } from '@angular/core';
import { AccountService } from 'src/app/services/network/account/account.service';
import { MatDialogRef } from '@angular/material/dialog';
import { ThemeService } from 'src/app/services/theme/theme.service';


@Component({
    selector: 'app-register',
    templateUrl: './register.component.html',
    styleUrls: ['./register.component.scss']
})
export class RegisterComponent implements OnInit {

    constructor(
        private accountService: AccountService,
        public theme: ThemeService,
        public dialogRef: MatDialogRef<RegisterComponent>) {
    }

    public user: {
        nickName: string,
        password: string,
        email: string,
        phoneNumber: string
    } = { nickName: "", password: "", email: "", phoneNumber: "" };
    ngOnInit(): void {

    }
    public get currentTheme() {
        return this.theme.getThemeClass();
    }
    public errorInfo: string = "";

    public isLoading: boolean = false;

    public register() {
        // if (this.user.password == "" || this.user.ni == "") {
        //     return this.errorInfo = "请输入您的信息";
        // }
        // this.isLoading = true;
        // this.accountService.login(this.user.username, this.user.password).subscribe(async res => {
        //     if (res.code != 200) {
        //         this.isLoading = false;
        //         this.errorInfo = res.message;
        //     } else {
        //         await this.accountService.getUserInfo().toPromise().then(() => {
        //             this.dialogRef.close();
        //             this.isLoading = false;
        //         });
        //     }
        // });
    }

    public onInputChange() {
        this.errorInfo = "";
    }
}