using System.ComponentModel.DataAnnotations;

namespace BlackFront.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название книги обязательно")]
        [Display(Name = "Название книги")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите автора или напишите 'Анонимно'")]
        [Display(Name = "Автор")]
        public string Author { get; set; } = string.Empty;

        [Display(Name = "Описание / Аннотация")]
        public string Description { get; set; } = string.Empty;

        public string? CoverPath { get; set; }

        [Display(Name = "Ссылка для чтения (необязательно)")]
        public string? ReadUrl { get; set; }

        public string? FilePath { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public string GetDisplayCover()
        {
            if (string.IsNullOrWhiteSpace(CoverPath))
            {
                return "/images/covers/cover-404.png";
            }
            return $"/uploads/covers/{CoverPath}";
        }

        public (string ActionUrl, bool IsDownload) GetFileAction()
        {
            if (!string.IsNullOrWhiteSpace(FilePath))
            {
                return ($"/uploads/books/{FilePath}", true);
            }
            if (!string.IsNullOrWhiteSpace(ReadUrl))
            {
                return (ReadUrl, false);
            }
            return ($"/Book?id={Id}", false);
        }
    }
}
