
import { Component } from '@angular/core';
import { MatSnackBarRef } from '@angular/material/snack-bar';
@Component({
    selector: 'app-snackBar',
    templateUrl: './snackBar.component.html',
    styleUrls: ['./snackBar.component.scss'],
})
export class SnackBarComponent {

    public constructor(public snackRef: MatSnackBarRef<SnackBarComponent>) {
        this.data = snackRef.containerInstance.snackBarConfig.data;
    }
    public data: string = "";
}
