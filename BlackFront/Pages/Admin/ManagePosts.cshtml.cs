using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.Packaging; 
using System.Text; 
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;


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
            
            NewPost.CreatedAt = DateTime.UtcNow;
            NewPost.UpdatedAt = null;

            if (NewPost.IsFeatured)
            {
                NewPost.FeaturedAt = DateTime.UtcNow;
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


        public async Task<IActionResult> OnPostParseFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return new JsonResult(new { success = false, error = "Файл пуст или не передан." });
            }

            var extension = System.IO.Path.GetExtension(file.FileName).ToLower().Replace(".", "");
            string extractedText = "";

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    switch (extension)
                    {
                        case "txt":
                        case "json":
                            using (var reader = new StreamReader(stream, Encoding.UTF8))
                            {
                                extractedText = await reader.ReadToEndAsync();
                            }
                            break;

                        case "docx":
                            using (DocumentFormat.OpenXml.Packaging.WordprocessingDocument wordDoc = 
                                DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(stream, false))
                            {
                                var body = wordDoc.MainDocumentPart.Document.Body;
                                extractedText = body.InnerText; 
                            }
                            break;

                        case "pdf":
                            using (PdfDocument document = PdfDocument.Open(stream))
                            {
                                var textBuilder = new StringBuilder();
                                foreach (var page in document.GetPages())
                                {
                                    textBuilder.AppendLine(page.Text);
                                }
                                extractedText = textBuilder.ToString();
                            }
                            break;

                        default:
                            return new JsonResult(new { success = false, error = "Формат файла не поддерживается сервером." });
                    }
                }

                return new JsonResult(new { success = true, content = extractedText });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, error = $"Не удалось прочитать файл: {ex.Message}" });
            }
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
