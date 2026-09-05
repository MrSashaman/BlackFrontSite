using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace YourProject.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditPostModel : PageModel
    {
        public List<Category> Categories { get; set; } = new();
        private readonly AppDbContext _context;
        public List<Badge> Badges { get; set; } = new();

        [BindProperty]
        public List<int> SelectedBadgeIds { get; set; } = new();
        public EditPostModel(AppDbContext context)
        {
            _context = context;
        }


        [BindProperty]
        public EditPostInput Input { get; set; } = new();


        public async Task<IActionResult> OnGetAsync(int id)
        {
            var post = await _context.Posts
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            Categories = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();
            Badges = await _context.Badges
                .Where(b => b.IsAdminOnly)
                .OrderBy(b => b.Id)
                .ToListAsync();


            SelectedBadgeIds = await _context.PostBadges
                .Where(pb => pb.PostId == id)
                .Select(pb => pb.BadgeId)
                .ToListAsync();
            if (post == null)
            {
                return NotFound();
            }


            Input = new EditPostInput
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CategoryId = post.CategoryId
            };


            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                Categories = await _context.Categories
                    .OrderBy(c => c.Name)
                    .ToListAsync();

                return Page();
            }


            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == Input.Id);
            var oldBadges = await _context.PostBadges
                .Where(pb => pb.PostId == post.Id)
                .ToListAsync();


            _context.PostBadges.RemoveRange(oldBadges);

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
                    PostId = post.Id,
                    BadgeId = badgeId
                });
            }

            post.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            
            if (post == null)
            {
                return NotFound();
            }


            post.Title = Input.Title.Trim();

            post.Content = Input.Content.Trim();

            post.CategoryId = Input.CategoryId;

            post.UpdatedAt = DateTime.UtcNow;


            await _context.SaveChangesAsync();


            return RedirectToPage("/Admin/ManagePosts");
        }


        public class EditPostInput
        {
            public int Id { get; set; }


            [Required]
            public string Title { get; set; } = string.Empty;


            [Required]
            public string Content { get; set; } = string.Empty;


            [Required]
            public int? CategoryId { get; set; }
        }
    }
}