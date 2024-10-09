using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CommentsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get(int bookId)
        {
            var bookExists = await _context.Books.AnyAsync(book => book.Id == bookId);

            if (!bookExists)
                return NotFound();

            var comments = await _context.Comments.Where(commentDb => commentDb.BookId == bookId).ToListAsync();
            return _mapper.Map<List<CommentDTO>>(comments);
        }

        [HttpGet("{id:int}", Name = "GetComment")]
        public async Task<ActionResult<CommentDTO>> GetById(int id)
        {
            var comment = await _context.Comments.FirstOrDefaultAsync(comment => comment.Id == id);

            if (comment == null)
                return NotFound();

            return _mapper.Map<CommentDTO>(comment);
        }

        [HttpPost]
        public async Task<ActionResult> Post(int bookId, CreateCommentDTO createdCommentDto)
        {
            var bookExists = await _context.Books.AnyAsync(book => book.Id == bookId);

            if (!bookExists)
                return NotFound();

            var comment = _mapper.Map<Comment>(createdCommentDto);
            comment.BookId = bookId;

            _context.Add(comment);
            await _context.SaveChangesAsync();
            var commentDto = _mapper.Map<CommentDTO>(comment);
            return CreatedAtRoute("GetComment", new { id = comment.Id, bookId }, commentDto);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int bookId, int id, CreateCommentDTO createdCommentDto)
        {
            var bookExists = await _context.Books.AnyAsync(book => book.Id == bookId);

            if (!bookExists)
                return NotFound();

            var exists = await _context.Comments.AnyAsync(comment => comment.Id == id);
            if (!exists) return NotFound();

            var comment = _mapper.Map<Comment>(createdCommentDto);
            comment.Id = id;
            comment.BookId = bookId;

            _context.Update(comment);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}