using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul postarii este obligatoriu !")]
        [MinLength(10, ErrorMessage = "Titlul postarii trebuie sa aiba minim 10 caractere")]
        [MaxLength(300, ErrorMessage = "Titlul postarii trebuie sa aiba maxim 300 de caractere")]
        public string PostName { get; set; }

        [Required(ErrorMessage = "Continutul postarii este obligatoriu !")]
        [MinLength(30, ErrorMessage = "Continutul postarii trebuie sa aiba minim 30 de caractere")]
        [MaxLength(500, ErrorMessage = "Continutul postarii trebuie sa aiba maxim 500 de caractere")]

        public string Content { get; set; }
        public DateTime Data { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; } 

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

    }
}
