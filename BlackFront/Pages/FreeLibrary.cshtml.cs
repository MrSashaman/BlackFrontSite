using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using BlackFront.Models;

namespace BlackFront.Pages
{
    public class FreeLibraryModel : PageModel
    {
        private readonly AppDbContext _context;

        public FreeLibraryModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Search { get; set; }

        public List<Book> FeaturedBooks { get; set; } = new();
        public List<Book> LatestBooks { get; set; } = new();

        public async Task OnGetAsync()
        {
            var booksQuery = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(Search))
            {
                string searchLower = Search.ToLower();
                booksQuery = booksQuery.Where(b => 
                    b.Title.ToLower().Contains(searchLower) || 
                    b.Author.ToLower().Contains(searchLower) || 
                    b.Description.ToLower().Contains(searchLower));
            }

            var allFilteredBooks = await booksQuery
                .OrderByDescending(b => b.AddedAt)
                .ToListAsync();

            if (string.IsNullOrWhiteSpace(Search))
            {
                FeaturedBooks = allFilteredBooks.Take(2).ToList();
                LatestBooks = allFilteredBooks.Skip(2).ToList();
            }
            else
            {
                FeaturedBooks = new List<Book>();
                LatestBooks = allFilteredBooks;
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                if (!string.IsNullOrEmpty(book.CoverPath))
                {
                    var coverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "covers", book.CoverPath);
                    if (System.IO.File.Exists(coverPath)) System.IO.File.Exists(coverPath);
                }
                if (!string.IsNullOrEmpty(book.FilePath))
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "books", book.FilePath);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Exists(filePath);
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

    }
}
