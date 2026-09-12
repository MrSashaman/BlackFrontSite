using Microsoft.EntityFrameworkCore;
using BlackFront.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Badge> Badges => Set<Badge>();

    public DbSet<PostBadge> PostBadges => Set<PostBadge>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Slug)
            .IsUnique();


        modelBuilder.Entity<Post>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Posts)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<PostBadge>()
            .HasKey(pb => new
            {
                pb.PostId,
                pb.BadgeId
            });


        modelBuilder.Entity<PostBadge>()
            .HasOne(pb => pb.Badge)
            .WithMany(b => b.PostBadges)
            .HasForeignKey(pb => pb.BadgeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>().HasData(

            new Category
            {
                Id = 1,
                Name = "Новости",
                Slug = "news"
            },

            new Category
            {
                Id = 2,
                Name = "История",
                Slug = "history"
            },

            new Category
            {
                Id = 3,
                Name = "Политическая теория",
                Slug = "political-theory"
            },

            new Category
            {
                Id = 4,
                Name = "Публицистика",
                Slug = "publicism"
            },

            new Category
            {
                Id = 5,
                Name = "Документы",
                Slug = "documents"
            },

            new Category
            {
                Id = 6,
                Name = "Творчество",
                Slug = "creation"
            }

        );

        modelBuilder.Entity<Badge>().HasData(

            new Badge
            {
                Id = 1,
                Emoji = "⭐",
                Name = "Рекомендует редакция",
                IsAdminOnly = true
            },

            new Badge
            {
                Id = 2,
                Emoji = "📜",
                Name = "Архив",
                IsAdminOnly = true
            },

            new Badge
            {
                Id = 3,
                Emoji = "✓",
                Name = "Проверено",
                IsAdminOnly = true
            },

            new Badge
            {
                Id = 4,
                Emoji = "✍",
                Name = "Авторский материал",
                IsAdminOnly = true
            },
            new Badge
            {
                Id = 5,
                Emoji = "🏴",
                Name = "Черный фронт",
                IsAdminOnly = true
            }

        );
    }
}