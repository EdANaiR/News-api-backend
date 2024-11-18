using Microsoft.EntityFrameworkCore;
using NewsAPI.Models.Entities;

namespace NewsAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } 
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // NewsImage için birincil anahtarı tanımlıyoruz
            modelBuilder.Entity<NewsImage>()
                .HasKey(ni => ni.ImageId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
