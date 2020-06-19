/// <reference path="./util/Validation.ts" />
import createError = require('http-errors');
import express = require('express');
import path = require('path');
import cookieParser = require('cookie-parser');
import logger = require('morgan');
import fs = require('fs');
import cheerio = require("cheerio");
import { request } from './util/request';
import * as StringHelper from './util/StringHelper';
import SongUrlSaver from './util/SongUrl';
import consul = require('consul');
const uuid = require('node-uuid');
const app = express();
const UrlSaver = new SongUrlSaver();
const Consul = new consul(
  { host: '172.18.0.2', port: 8500, promisify: true }
);


setInterval(() => UrlSaver.write(), 3600000 * 3);

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');

app.use(logger('dev'));
app.use(express.json());
app.use(express.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

fs.readdirSync(path.join(__dirname, 'routes')).reverse().forEach(file => {
  const filename = file.replace(/(\.js|\.ts)$/, '');
  app.use(`/${filename}`, (req, res, next) => {
    const router = express.Router();
    req.query = {
      ...req.query,
      ...req.body,
    };
    const RouterMap: Validation.RouterMap = require(`./routes/${filename}`);
    Object.keys(RouterMap).forEach((path: string): void => {
      let rObj = RouterMap[path];
      if (typeof rObj === 'function') {
        rObj = {
          func: rObj,
          post: true,
          get: true,
        }
      }
      const func = (req, res, next) => rObj.func({
        req,
        res,
        next,
        request: new request({ req, res, next }),
        cheerio,
        ...StringHelper,
        UrlSaver,
      });
      if (rObj.post) {
        router.post(path, func);
      }
      if (rObj.get) {
        router.get(path, func);
      }
    });
    router(req, res, next);
  });
});

app.use('/', require('./routes/index'));

// catch 404 and forward to error handler
app.use(function (req, res, next) {
  next(createError(404));
});
let id = "service:" + uuid.v1();
Consul.agent.service.deregister(
  id, (err) => {
    if (err) throw err;
  }
);

  Consul.agent.service.register({
    name: "Migu API", address: '172.18.0.8', port: 3500, id: id,
    check: {
      http: 'http://172.18.0.8:3500/health', interval: '10s', timeout: '5s',
      deregisterCriticalServiceAfter: '60s'
    }
  }, function (err, result) {
    if (err) { console.error(err); }
    console.log("MiguAPI" + ' 注册成功！');
  });
// error handler
app.use(function (err, req, res, next) {
  // set locals, only providing error in development
  res.locals.message = err.message;
  res.locals.error = req.app.get('env') === 'development' ? err : {};

  // render the error page
  res.status(err.status || 500);
  res.render('error');
});

module.exports = app;
