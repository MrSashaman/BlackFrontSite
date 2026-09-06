public class Post
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    public int? CategoryId { get; set; }

    public Category? Category { get; set; }

    public List<PostBadge> PostBadges { get; set; } = new();

    public bool IsFeatured { get; set; } = false;

    public DateTime? FeaturedAt { get; set; }
}