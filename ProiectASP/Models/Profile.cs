using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    public class Profile
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Prenumele este obligatoriu !")]
        public string First_Name { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu !")]
        public string Last_Name { get; set;}

        [Required(ErrorMessage = "Descrierea este obligatorie !")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Job-ul este obligatoriu !")]
        public string Job { get; set; }

        [MinLength(10, ErrorMessage = "Numarul de telefon trebuie sa aiba 10 caractere")]
        [MaxLength(10, ErrorMessage = "Numarul de telefon trebuie sa aiba 10 caractere")]
        [Required(ErrorMessage = "Numarul de telefon este obligatoriu !")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Fotografia de profil este obligatorie !")]
        public string? Image { get; set; }

        public Status Profile_Status { get; set; }

        public string? UserId { get; set; }

        public virtual ApplicationUser? User { get; set; }

        public virtual ICollection<ProfileGroup>? ProfileGroups { get; set; }

        public virtual ICollection<Friend>? Friends { get; set; }

    }

    public enum Status
    { 
        Public, Privat
    }

}
