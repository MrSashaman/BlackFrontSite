public class PostBadge
{
    public int PostId { get; set; }

    public Post Post { get; set; } = null!;


    public int BadgeId { get; set; }

    public Badge Badge { get; set; } = null!;
}