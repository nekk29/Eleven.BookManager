using System.ComponentModel.DataAnnotations;

namespace Eleven.BookManager.Entity.Base
{
    public class SystemEntity
    {
        [Required]
        [MaxLength(64)]
        public string CreationUser { get; set; } = null!;

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        [MaxLength(64)]
        public string UpdateUser { get; set; } = null!;

        [Required]
        public DateTime UpdateDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
