using AuthorsWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace AuthorsWebApi.Entities
{
    public class Author
    {
        public int Id { get; set; }
        [Required, MinLength(3), MaxLength(120)]
        [FirstCapitalizeLetter]
        public string Name { get; set; }
        public List<Book> Books { get; set; }
        public List<BookAuthor> BookAuthors { get; set; }
    }
}