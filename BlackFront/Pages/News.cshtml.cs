using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class NewsModel : PageModel
{
    private readonly AppDbContext _context;

    public NewsModel(AppDbContext context)
    {
        _context = context;
    }

    public List<Post> Posts { get; set; } = new();

    public async Task OnGetAsync()
    {
        Posts = await _context.Posts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }
}