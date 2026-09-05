public class Badge
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Emoji { get; set; } = string.Empty;

    public bool IsAdminOnly { get; set; } = true;

    public List<PostBadge> PostBadges { get; set; } = new();
}