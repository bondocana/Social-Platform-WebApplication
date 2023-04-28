using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectASP.Models
{
    public class ProfileGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ProfileId { get; set; }
        public int? GroupId { get; set; }

        public virtual Profile? Profile { get; set; }
        public virtual Group? Group { get; set; }

    }
}
