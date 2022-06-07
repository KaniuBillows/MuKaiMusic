FROM node:12 as builder
#WORKDIR 指令设置了工作目录的位置，意思就是进入到 /usr/src/app 这个目录，然后要在这个目录里去做一些事情。这里的目录是在镜像里的一个位置。
WORKDIR /usr/src/app

#COPY 指令可以复制本地主机上的文件到镜像里，第一个点指的是 Dockerfile 文件所在的目录，这个目录是本地主机上的位置。第二个点指的是镜像里的当前目录，因为之前用 WORKDIR 设置了工作目录的位置，所以第二个点在这里指的就是镜像里的 /usr/src/app 这个目录。

#这一步做的事情就是把在本地上的 Angular 应用复制到镜像里面。
COPY ./Angular .

#运行了一下 npm install 命令，也就是安装 Angular 项目需要的所有的东西
RUN npm install
#它运行的是 ng build --prod，作用就是构建一个适合在生产环境上运行的 Angular 应用

RUN ./node_modules/.bin/ng build --prod
#这里的nginx版本可以去掉 就会下载 latest
FROM nginx

RUN 'ls'
#你需要将这里的angulardemo10换成你执行ng build在dist下生成的目录 一般是你的项目名称
COPY --from=builder /usr/src/app/dist /usr/share/nginx/html
#设置用户标签
LABEL maintainer="mukaiMusic"
#这是将你配置好的nginx配置替换docker里默认的nginx 建议学习nginx
COPY ./Angular/nginx-angular.conf /etc/nginx/conf.d/default.conf