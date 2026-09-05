using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace YourProject.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class ManagePostsModel : PageModel
    {
        public List<Category> Categories { get; set; } = new();

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
                .Include(p => p.Category)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();


            Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (NewPost.CategoryId == null ||
                !await _context.Categories
                    .AnyAsync(c => c.Id == NewPost.CategoryId))
            {
                ModelState.AddModelError(
                    "NewPost.CategoryId",
                    "Выберите категорию.");
            }


            if (!ModelState.IsValid)
            {
                Posts = await _context.Posts
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToListAsync();

                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
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