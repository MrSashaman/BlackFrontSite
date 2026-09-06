using Microsoft.AspNetCore.Mvc;
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


    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }


    public async Task OnGetAsync()
    {
        var query = _context.Posts
            .Include(p => p.Category)
            .Include(p => p.PostBadges)
                .ThenInclude(pb => pb.Badge)
            .AsNoTracking()
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(Search))
        {
            var search = Search.Trim();

            query = query.Where(p =>
                EF.Functions.Like(
                    p.Title,
                    $"%{search}%"
                )
                ||
                EF.Functions.Like(
                    p.Content,
                    $"%{search}%"
                )
            );
        }


        FeaturedPosts = await query
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.FeaturedAt)
            .ToListAsync();


        LatestPosts = await query
            .Where(p => !p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}