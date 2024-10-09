using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Entities
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        [FirstCapitalizeLetter]
        [StringLength(maximumLength: 250)]
        public string Title { get; set; }
        public DateTime? PublicationDate { get; set; }
        public List<Comment> Comments { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }
    }
}