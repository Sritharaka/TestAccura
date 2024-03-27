using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestAccura.Models
{
    [Table("User", Schema = "Accura")]
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Password { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? DeleteDate { get; set; }

    }
}
