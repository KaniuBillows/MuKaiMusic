let reqst = require("request");

module.exports = {
  async ['/']({ req, res, request }) {
    const { id } = req.query;

    if (!id) {
      return res.send({
        code: 400,
        message: '参数错误',
        content: null
      })
    }
    const settings = {
      method: 'get',
      url: `http://music.migu.cn/v3/api/music/audioPlayer/getLyric?copyrightId=${id}`,
      headers: {
        'Referer': 'http://music.migu.cn/v3/music/player/audio',
        'Host': 'music.migu.cn'
      }
    }

    console.log(settings.url)

    // const result = await request.send(`http://music.migu.cn/v3/api/music/audioPlayer/getLyric?copyrightId=${id}`);
    reqst(settings, (err, _res, body) => {
      if (err) {
        console.error(err);
      }
      let result = JSON.parse(body);
      if (result.msg === '成功') {
        let pattern = /^\[[0-9]{2}\:[0-9]{2}\.[0-9]{2}\]/;
        let strs: string[] = result.lyric.split("\n");
        let lyrics: {
          time: number,
          text: string
        }[] = strs.map(str => {
          let r: { time: number, text: string } = {} as any;
          if (pattern.test(str)) {
            let timeString: string = str.match(pattern)[0];
            timeString = timeString.substring(1, timeString.length - 2);
            r.time = Number.parseInt(timeString.substr(0, 2)) * 60
              + Number.parseFloat(timeString.substring(timeString.indexOf(":") + 1, timeString.length));
            r.text = str.replace(str.match(pattern)[0], "");
          } else {
            r.text = str;
          }
          return r;
        });


        return res.send({
          code: 200,
          content: lyrics,
          message: null
        });
      }
      return res.send({
        code: 500,
        contet: null,
        message: result,
      });
    });

  },
};
