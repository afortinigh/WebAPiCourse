using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;

namespace AuthorsWebApi.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<NewAuthorDTO, Author>();
            CreateMap<Author, AuthorDTO>();
            CreateMap<Author, AuthorDTOWithBooks>().ForMember(authorDto => authorDto.Books, options => options.MapFrom(AuthorsBookDtoMap)); ;
            CreateMap<CreateBookDTO, Book>().ForMember(book => book.BookAuthors, options => options.MapFrom(BooksAuthorMap));
            CreateMap<Book, BookDTOWithAuthors>();
            CreateMap<Book, BookDTOWithAuthors>().ForMember(bookDto => bookDto.Authors, options => options.MapFrom(BookAuthorsDtoMap));
            CreateMap<PatchBookDTO, Book>().ReverseMap();
            CreateMap<CreateCommentDTO, Comment>();
            CreateMap<Comment, CommentDTO>();
        }

        private List<BookAuthor> BooksAuthorMap(CreateBookDTO bookDTO, Book book)
        {
            var result = new List<BookAuthor>();

            if (bookDTO.AuthorsId == null)
                return result;

            foreach (var authorId in bookDTO.AuthorsId)
                result.Add(new BookAuthor() { AuthorId = authorId });

            return result;
        }

        private List<AuthorDTO> BookAuthorsDtoMap(Book book, BookDTO bookDto)
        {
            var result = new List<AuthorDTO>();

            if (book.BookAuthors == null) return result;

            foreach (var bookAuthor in book.BookAuthors)
            {
                result.Add(new AuthorDTO()
                {
                    Id = bookAuthor.AuthorId,
                    Name = bookAuthor.Author.Name
                });
            }

            return result;
        }

        private List<BookDTO> AuthorsBookDtoMap(Author author, AuthorDTO authorDto)
        {
            var result = new List<BookDTO>();

            if (author.BookAuthors == null) return result;

            foreach (var authorBook in author.BookAuthors)
            {
                result.Add(new BookDTO()
                {
                    Id = authorBook.BookId,
                    Title = authorBook.Book.Title
                });
            }

            return result;
        }
    }
}