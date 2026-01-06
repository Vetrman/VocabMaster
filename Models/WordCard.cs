using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VocabMaster.Models
{
    public class WordCard
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Original Word")]
        public string OriginalWord { get; set; } = "";

        [Required]
        [Display(Name = "Translation")]
        public string Translation { get; set; } = "";

        [Display(Name = "Example Sentence")]
        public string? ExampleSentence { get; set; }

        [Display(Name = "Image Path")]
        public string? ImagePath { get; set; } // Путь к картинке

        [Display(Name = "From Language")]
        public string LanguageFrom { get; set; } = "en";

        [Display(Name = "To Language")]
        public string LanguageTo { get; set; } = "ru";

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public string? UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual Microsoft.AspNetCore.Identity.IdentityUser? User { get; set; }

        // Для загрузки файла (не хранится в БД)
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
    }
}