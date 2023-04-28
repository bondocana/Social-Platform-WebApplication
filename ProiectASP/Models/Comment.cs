using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Continutul comentariului este obligatoriu !")]        
        [MaxLength(300, ErrorMessage = "Continutul comentariului trebuie sa aiba maxim 300 de caractere")]
        public string Text { get; set; }

        public DateTime Data { get; set; }

        public int? PostId { get; set; }

        public virtual Post? Postare { get; set; }

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }


    }
}
