using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.DTOs
{
    public class CreateBookDTO
    {
        [FirstCapitalizeLetter]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime PublicationDate { get; set; }
        public List<int> AuthorsId { get; set; }
    }
}