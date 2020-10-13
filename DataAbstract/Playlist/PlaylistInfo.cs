using System.Collections.Generic;

namespace DataAbstract.Playlist
{
    /// <summary>
    /// 歌单信息
    /// </summary>
    public class PlaylistInfo
    {
        public DataSource DataSource { get; set; }

        public long Id { get; set; }

        public string Name { get; set; }

        public string PicUrl { get; set; }

        public int MusicCount { get; set; }

        public List<MusicInfo> Musics { get; set; } = new List<MusicInfo>();
    }
}