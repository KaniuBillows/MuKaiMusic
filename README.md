# MuKaiMusic
一个免费的音乐平台，致力于：一个平台 所有音乐

目前功能正在缓慢开发中

支持音乐搜索，播放

播放列表将被存储在客户端localStorage中

# DEMO 地址:
https://music.kaniu.pro

# 自己部署
本项目包含几个不同的模块

基于nodeJs的[migu API](https://github.com/KaniuBillows/MiguMusicApi)，来自[jsososo](https://github.com/jsososo)

基于asp.net core的[Kuwo API](https://github.com/KaniuBillows/KuwoMusicAPI)

基于asp.net core的[Netease API](https://github.com/KaniuBillows/NeteaseMusicAPI) 移植自[NeteaseCloudMusicApi](https://github.com/Binaryify/NeteaseCloudMusicApi)

基于asp.net core与Grpc的[账户功能模块](https://github.com/KaniuBillows/MuKai-Account) 

如果你需要全部的功能，你需要部署全部以上的服务。或者你可以只选取你所需要的功能模块。

## 配置文件
在/MuKai Music/下新建appsettings.json
它应该如下所示：
```
{
 "urls": "http://*:2000",
  "cache-age": 86400, //服务器缓存默认过期时间
  "cache-type": "memory"||"redis",//服务器缓存类型，支持memory和redis
  "SecurityKey": "",//JWT 加密密钥
  "PrivateKey": "-----BEGIN PRIVATE KEY-----XXXX-----END PRIVATE KEY-----",//RSA 加密密钥
  "Domain": "https://music.kaniu.pro",//JWT domain
  "Expires": 120,// Access Token过期时间，分钟
  "RefreshTime": 7, // Refresh Token过期时间 天
  "ConnectionStrings": {
    "Redis": "",// Redis地址,
    "MongoDB": "" // MongoDB 地址
  },//数据库链接字符串
   "MiguAPI": "",//咪咕音乐API地址和端口
  "KuwoAPI": "",//酷我音乐API地址和端口
  "NeAPI": "",//网易云音乐API地址和端口
  "AccountAddredss": "",//账户服务地址和端口
  "PicRoot": ""图片上传路径
 }
```
然后执行运行应用程序。


