using System;

namespace MuKai_Music.Model.ResponseEntity.SearchResult
{

    public sealed class SearchMusic : ProcessedData
    {
        public SearchMusic(DataSource type) : base(type)
        {

        }


        public string Name { get; set; }

        public Artist Aritst { get; set; }

        public Album Album { get; set; }
    }

    public sealed class Artist : ProcessedData
    {
        public Artist(DataSource type) : base(type)
        {
        }

        public string Name { get; set; }

        public string Pic { get; set; }
    }

    public sealed class Album : ProcessedData
    {
        public Album(DataSource searchType) : base(searchType)
        {
        }

        public string Name { get; set; }

        public string Pic { get; set; }
    }

 
}
