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
        private readonly AppDbContext _context;

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

            if (post == null)
            {
                return NotFound();
            }


            Input = new EditPostInput
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content
            };


            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            var post = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == Input.Id);

            if (post == null)
            {
                return NotFound();
            }


            post.Title = Input.Title.Trim();
            post.Content = Input.Content.Trim();

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
        }
    }
}