using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class NewsModel : PageModel
{
    private readonly AppDbContext _context;

    public NewsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Post> FeaturedPosts { get; set; } = new();

    public List<Post> LatestPosts { get; set; } = new();

    public async Task OnGetAsync()
    {
        FeaturedPosts = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.PostBadges)
                .ThenInclude(pb => pb.Badge)
            .Where(p => p.IsFeatured)
            .AsNoTracking()
            .OrderByDescending(p => p.FeaturedAt)
            .ToListAsync();


        LatestPosts = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.PostBadges)
                .ThenInclude(pb => pb.Badge)
            .Where(p => !p.IsFeatured)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}