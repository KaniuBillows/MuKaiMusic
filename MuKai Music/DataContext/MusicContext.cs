using Microsoft.EntityFrameworkCore;
using MuKai_Music.Model.DataEntity;

namespace MuKai_Music.DataContext
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options)
        {
        }

        public DbSet<MusicInfo> Migu_Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MusicInfo>().ToTable("MusicInfo");
        }
    }
}