FROM node:12 as sdk

WORKDIR /app
RUN npm config set registry "https://registry.npm.taobao.org/"

RUN  npm install -g @angular/cli@10

COPY ./package.json /app

RUN npm install

FROM node:12 as builder

WORKDIR /app

COPY --from=sdk /app /app

RUN npm run build

FROM nginx:latest

WORKDIR /app

RUN rm -rf /usr/share/nginx/html/*

COPY --from=builder /app/dist/mukaiMusic /usr/share/nginx/html

EXPOSE 80

ENTRYPOINT /bin/sh -c "nginx -g `daemon off;`"