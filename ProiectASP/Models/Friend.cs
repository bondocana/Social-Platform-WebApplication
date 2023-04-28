using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ProiectASP.Models
{
    [Authorize]
    public class Friend
    {
        [Key]
        public int Id { get; set; }

        public int? ProfileId { get; set; }

        public virtual Profile? Profile { get; set; }

        public string? UserId { get; set; }

        // PASUL 6 - useri si roluri
        public virtual ApplicationUser? User { get; set; }

    }
}
