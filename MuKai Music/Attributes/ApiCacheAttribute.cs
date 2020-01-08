using System;

namespace MuKai_Music.Attribute
{
    public sealed class ApiCacheAttribute : System.Attribute
    {
        public long Duration { get; set; }

        public bool NoStore { get; set; }
    }
}
