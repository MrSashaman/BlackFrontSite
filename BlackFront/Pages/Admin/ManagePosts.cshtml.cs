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
        public List<Badge> Badges { get; set; } = new();

        [BindProperty]
        public List<int> SelectedBadgeIds { get; set; } = new();

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
            
            Badges = await _context.Badges
                .Where(b => b.IsAdminOnly)
                .OrderBy(b => b.Id)
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

            var validBadgeIds = await _context.Badges
                .Where(b =>
                    SelectedBadgeIds.Contains(b.Id) &&
                    b.IsAdminOnly)
                .Select(b => b.Id)
                .ToListAsync();


            foreach (var badgeId in validBadgeIds)
            {
                _context.PostBadges.Add(new PostBadge
                {
                    PostId = NewPost.Id,
                    BadgeId = badgeId
                });
            }


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