import { Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class EditDistanceService {
    public constructor() {

    }

    public getEditDistance(keyword: string, target: string): number {
        let n = keyword.length;
        let m = target.length;
        if (m * n == 0) return m + n;
        let dp = new Array(n + 1).fill(0).map(() => new Array(m + 1).fill(0));
        for (let i = 0; i < n + 1; i++) {
            dp[i][0] = i;
        }
        for (let j = 0; j < m + 1; j++) {
            dp[0][j] = j;
        }
        for (let i = 1; i < n + 1; i++) {
            for (let j = 1; j < m + 1; j++) {
                let left = dp[i - 1][j] + 1;
                let down = dp[i][j - 1] + 1;
                let left_down = dp[i - 1][j - 1];
                if (keyword.charAt(i - 1) != target.charAt(j - 1))
                    left_down += 1;
                dp[i][j] = Math.min(left, Math.min(down, left_down));
            }
        }
        return dp[n][m];
    }
}