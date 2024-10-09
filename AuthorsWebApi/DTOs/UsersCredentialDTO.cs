using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class UsersCredentialDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}