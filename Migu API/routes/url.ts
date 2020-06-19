module.exports = {
    async ['/']({ req, res, request }) {
        const { id, type = '128', isRedirect = '0' } = req.query;
        if (!id) {
            return res.send({
                code: 400,
                message: '有参数没传呀小老弟',
                content: null
            })
        }

        const result = await request.send({
            url: 'http://app.c.nf.migu.cn/MIGUM2.0/v2.0/content/listen-url',
            data: {
                netType: '01',
                resourceType: 'E',
                songId: id,
                toneFlag: {
                    128: 'PQ',
                    320: 'HQ',
                    flac: 'SQ',
                }[type],
                dataType: 2,
                // data: c,
                // secKey: f,
            },
            headers: {
                referer: 'http://music.migu.cn/v3/music/player/audio',
                channel: '0146951',
                uid: 1234,
            }
        });
        console.log(result);
        if (result.code === '000000') {
            if (Number(isRedirect)) {
                return res.redirect(result.data.url);
            }
            return res.send({
                code: 200,
                content: result.data.url,
                message: 'success'
            });
        } else {
            return res.send({
                code: 200,
                content: result.info,
                message: 'success'
            })
        }
    }
}