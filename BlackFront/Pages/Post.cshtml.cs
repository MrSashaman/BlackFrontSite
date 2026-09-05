using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

public class PostModel : PageModel
{
    private readonly AppDbContext _context;

    public PostModel(AppDbContext context)
    {
        _context = context;
    }

    public Post Post { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var post = await _context.Posts

            .Include(p => p.Category)

            .AsNoTracking()

            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null)
        {
            return NotFound();
        }

        Post = post;

        return Page();
    }
}