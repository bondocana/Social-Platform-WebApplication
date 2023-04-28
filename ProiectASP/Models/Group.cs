using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele grupului este obligatoriu !")]
        public string GroupName { get; set; }

        [Required(ErrorMessage = "Subiectul grupului este obligatoriu !")]
        public string Subject { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<ProfileGroup>? ProfileGroups { get; set; }



        public virtual ICollection<Chat>? Chats { get; set; }



    }
}
