export class ExpireContent<T>{
    constructor(option: { content: T, expire: Date }) {
        this.content = option.content;
        this.expire = option.expire;
    }
    content: T;
    expire: Date
}