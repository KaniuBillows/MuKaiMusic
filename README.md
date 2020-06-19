# MuKaiMusic with MicroService
一个免费的音乐平台，致力于：一个平台 所有音乐。
本项目只是个人学习开发用的，任何人，任何组织不得以任何理由商用。

# 微服务架构
这一段时间并没有添加新的功能，但是并没有偷懒。正在学习全新的微服务架构，奈何手上没啥项目，那就好拿Mukai Music开刀了。
本人才疏学浅，也是第一次玩这个，如果能帮到你，我非常高兴。如果做的不好，希望能得到建议。

## 项目基于原Mukai Music重构而来

### 使用Docker，更加易于部署（我的数据库用的RDS（便宜），不然直接放Docker里面了，会更方便）
### Ocelot网关，咱.NET就是好啊
### Consul服务治理，微服务必备
### 完全解耦各个模块，但为了保证数据格式同一，任然保留DataAstract项目，其中定义了数据结构（各个音乐平台数据结构太不一样了，不同一很麻烦）
### JWT验证方案，单独的验证服务，单点登录必备（但是注册账户、用户资料还没来得及搞，哈哈哈）
### 整合三个音乐平台，基本能听许多许多歌曲了
### 不想每次都搜索，生成字节的歌单（已开发完成API，客户端正在开发，详见Mukai Playlist）

# 如何部署（如果部署遇到问题，可以试试联系我，或许咱能帮你呢）
0. 一个Docker，一个RDS（验证用），一个MongoDB（自定义歌单用）
1. 配置Docker网络，新建名为“mynet”的network，并将其设置为172.18.0.0/16（当然你完全可以选择设置其他网段，但是你得更改所有项目的相关配置文件建议你还是照做）
2. 通过Consul路径下的docker-compose.yml，启动Consul，默认包含一个Server和Client。当然，你可以修改它。
3. 在保证Consul已经Ok的情况下，其他的服务你可以以任意顺序启动（docker-compose.yml）。
4. 如果你在服务文件夹里发现了build.cmd,或者build.sh，那么麻烦在用docker-compose之前，先运行它。（因为这服务需要DataAbstarct中定义的数据结构，但是DataAbstract并不在Docker上下文中，所以我们直接构建应用程序，并复制到Docker镜像中）
5. Mukai Auth，需要一个关系型数据库(我是postgresql)(你可以在它的配置文件中设置对应的连接字符串（appsettings.{Environment}.json）然后用对应的EF Core工具进行迁移，记得修改Startup.cs)。
6. Mukai Playlist需要一个MongoDB,需要在appsettings.{Environment}.json中进行配置。

## 部署步骤写的很详细，目的是帮助你更加熟悉它。你或许也可以试试用一个docker-compose.yml搞定所有。

# 服务结构：
![结构](https://kaniu-pic.oss-cn-chengdu.aliyuncs.com/githubPic/Architecture.png)
