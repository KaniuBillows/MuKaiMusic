module.exports = {
    async ['/']({ req, res, request }) {
        const id = req.query.id;
        if (!id) {
            return res.send({
                code: 400,
                message: '参数错误',
                content: null
            })
        }
        
        const result = await request.send(`http://music.migu.cn/v3/api/music/audioPlayer/getSongPic?songId=${id}`);
        if (result.msg === '成功') {
            res.send({
                code: 200,
                content: result.smallPic,
                message: null
            });
        } else {
            res.send({
                code: 500,
                contet: null,
                message: result.msg,
            });
        }
    }
};