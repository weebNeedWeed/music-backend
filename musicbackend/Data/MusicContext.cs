using Microsoft.EntityFrameworkCore;
using musicbackend.Models;

namespace musicbackend.Data
{
    public class MusicContext : DbContext
    {
        public MusicContext(DbContextOptions<MusicContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Music> Musics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Music>()
                .HasOne(s => s.User)
                .WithMany(g => g.Musics)
                .HasForeignKey(k => k.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
