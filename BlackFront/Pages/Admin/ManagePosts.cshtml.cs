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
            Posts = await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Posts = await _context.Posts.OrderByDescending(p => p.CreatedAt).ToListAsync();
                return Page();
            }

            _context.Posts.Add(NewPost);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
