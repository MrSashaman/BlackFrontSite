using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


    public DbSet<Post> Posts => Set<Post>();

    public DbSet<Category> Categories => Set<Category>();


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
    }
}