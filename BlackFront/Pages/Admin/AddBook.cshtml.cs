using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BlackFront.Models;

namespace BlackFront.Pages.Admin
{
    public class AddBookModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AddBookModel(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Book NewBook { get; set; } = new();

        [BindProperty]
        public IFormFile? CoverFile { get; set; }

        [BindProperty]
        public IFormFile? BookFile { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (CoverFile != null && CoverFile.Length > 0)
            {
                string uniqueCoverName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(CoverFile.FileName);
                string coversFolder = Path.Combine(_environment.WebRootPath, "uploads", "covers");
                if (!Directory.Exists(coversFolder)) Directory.CreateDirectory(coversFolder);
                
                using (var fileStream = new FileStream(Path.Combine(coversFolder, uniqueCoverName), FileMode.Create))
                {
                    await CoverFile.CopyToAsync(fileStream);
                }
                NewBook.CoverPath = uniqueCoverName;
            }

            if (BookFile != null && BookFile.Length > 0)
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(BookFile.FileName);
                string booksFolder = Path.Combine(_environment.WebRootPath, "uploads", "books");
                if (!Directory.Exists(booksFolder)) Directory.CreateDirectory(booksFolder);

                using (var fileStream = new FileStream(Path.Combine(booksFolder, uniqueFileName), FileMode.Create))
                {
                    await BookFile.CopyToAsync(fileStream);
                }
                NewBook.FilePath = uniqueFileName;
            }

            _context.Books.Add(NewBook);
            await _context.SaveChangesAsync();

            return RedirectToPage("/FreeLibrary");
        }
    }
}
