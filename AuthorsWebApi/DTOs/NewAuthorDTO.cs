using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class NewAuthorDTO
    {
        [Required, MinLength(3), MaxLength(120)]
        [FirstCapitalizeLetter] 
        public string Name { get; set; }
    }
}