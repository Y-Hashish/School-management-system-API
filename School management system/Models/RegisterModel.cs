using System.ComponentModel.DataAnnotations;

namespace School_management_system.Models
{
    public class RegisterModel
    {
        [Required,MaxLength(50)]
        public string FirstName { get; set; }
        [Required, MaxLength(50)]
        public string LastName { get; set; }
        [Required, MaxLength(50)]
        public string UserName { get; set; }
        [Required, MaxLength(100)]
        public string Password { get; set; }
        [Required, MaxLength(250)]
        public string Email { get; set; }

    }
}
