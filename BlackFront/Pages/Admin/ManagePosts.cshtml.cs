using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace YourProject.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManagePostsModel : PageModel
    {
        private readonly AppDbContext _context;

        public ManagePostsModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Post> Posts { get; set; } = new();

        [BindProperty]
        public Post NewPost { get; set; } = new();


        public async Task OnGetAsync()
        {
            Posts = await _context.Posts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Posts = await _context.Posts
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                return Page();
            }

            NewPost.CreatedAt = DateTime.UtcNow;
            NewPost.UpdatedAt = null;

            _context.Posts.Add(NewPost);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}