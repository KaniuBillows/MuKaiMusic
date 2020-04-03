import { Injectable, EventEmitter } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class ThemeService {
    constructor() {
        this.themes = [];
        this.themes.push('cyan-theme');
        this.themes.push('light_blue-theme');
        this.themes.push('blue-theme');
        this.themes.push('purple-theme');
        this.themes.push('pink-theme');
        this.themes.push('teal-theme');
        this.themes.push('green-theme');
        this.themes.push('light_green-theme');
        this.themes.push('lime-theme');
        this.themes.push('yellow-theme');
        this.themes.push('amber-theme');
        this.themes.push('orange-theme');
    }
    public themes: string[];
    private _themeclas: string = 'blue-theme';
    /**
     * 设置当前主题class
     */
    public setThemeClass(theme: string) {
        this._themeclas = theme;
    }

    /**
     * 获取当前主题class
     */
    public getThemeClass(): string {
        return this._themeclas;

    }

    // /**
    //  * 获取非Mat组件应设置的class
    //  */
    // public getThemeColor(): string {

    // }
}