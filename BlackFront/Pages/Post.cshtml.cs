using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class PostModel : PageModel
{
    private readonly AppDbContext _context;
    public List<PostBadge> PostBadges { get; set; } = new();
    public List<Post> RecommendedPosts { get; set; } = new();

    public PostModel(AppDbContext context)
    {
        _context = context;
    }

    public Post Post { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var post = await _context.Posts
            .Include(p => p.Category)
            .Include(p => p.PostBadges)
                .ThenInclude(pb => pb.Badge)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        Post = post;

        if (post.CategoryId.HasValue)
        {
            RecommendedPosts = await _context.Posts
                .Include(p => p.Category)
                .Where(p =>
                    p.Id != post.Id &&
                    p.CategoryId == post.CategoryId)
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .Take(4)
                .AsNoTracking()
                .ToListAsync();
        }


        if (RecommendedPosts.Count < 4)
        {
            var existingIds = RecommendedPosts
                .Select(p => p.Id)
                .ToList();

            existingIds.Add(post.Id);

            var additionalPosts = await _context.Posts
                .Include(p => p.Category)
                .Where(p => !existingIds.Contains(p.Id))
                .OrderByDescending(p => p.IsFeatured)
                .ThenByDescending(p => p.CreatedAt)
                .Take(4 - RecommendedPosts.Count)
                .AsNoTracking()
                .ToListAsync();

            RecommendedPosts.AddRange(additionalPosts);
        }


        return Page();
    }
}