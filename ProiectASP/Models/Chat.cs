using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    public class Chat
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nu puteti adauga un chat gol !")]
        public string Text { get; set; }

        public int? GroupId { get; set; }

        public virtual Group? Group { get; set; }



        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }
    }
}
