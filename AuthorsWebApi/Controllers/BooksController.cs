using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public BooksController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "GetBook")]
        public async Task<ActionResult<BookDTOWithAuthors>> Get(int id)
        {
            var book = await _dbContext.Books
                                       .Include(x => x.BookAuthors)
                                       .ThenInclude(x => x.Author)
                                       .FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
                return NotFound();

            book.BookAuthors = book.BookAuthors.OrderBy(x => x.Order).ToList();

            return _mapper.Map<BookDTOWithAuthors>(book);
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateBookDTO createdBookDto)
        {
            if (createdBookDto.AuthorsId == null)
                return BadRequest("You cannot create a Book withouth Authors");

            var authorsId = await _dbContext.Authors.Where(x => createdBookDto.AuthorsId.Contains(x.Id))
                                                    .Select(x => x.Id)
                                                    .ToListAsync();

            if (createdBookDto.AuthorsId.Count != authorsId.Count)
                return BadRequest("Do not exist one of the Authors sent");

            var book = _mapper.Map<Book>(createdBookDto);
            AssignAuthorsOrder(book);

            _dbContext.Add(book);
            await _dbContext.SaveChangesAsync();
            var bookDto = _mapper.Map<Book>(book);
            return CreatedAtRoute("GetBook", new { id = book.Id }, bookDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CreateBookDTO createdBookDto)
        {
            var bookDb = await _dbContext.Books.Include(x => x.BookAuthors)
                                               .FirstOrDefaultAsync(x => x.Id == id);

            if (bookDb == null)
                return NotFound();

            bookDb = _mapper.Map(createdBookDto, bookDb);
            AssignAuthorsOrder(bookDb);

            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<PatchBookDTO> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();

            var bookDb = await _dbContext.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (bookDb == null)
                return NotFound();

            var bookDto = _mapper.Map<PatchBookDTO>(bookDb);

            patchDocument.ApplyTo(bookDto, ModelState);

            var valid = TryValidateModel(bookDto);

            if (!valid)
                return BadRequest(ModelState);

            _mapper.Map(bookDto, bookDb);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await _dbContext.Books.AnyAsync(x => x.Id == id);

            if (!exists)
                return NotFound();

            _dbContext.Remove(new Book() { Id = id });
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        private void AssignAuthorsOrder(Book book)
        {
            if (book.BookAuthors != null)
            {
                for (int i = 0; i < book.BookAuthors.Count; i++)
                {
                    book.BookAuthors[i].Order = i;
                }
            }
        }
    }
}