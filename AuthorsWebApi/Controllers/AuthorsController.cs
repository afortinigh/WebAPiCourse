using AuthorsWebApi.DTOs;
using AuthorsWebApi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthorsWebApi.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public AuthorsController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<List<AuthorDTO>> Get()
        {
            var authors = await _dbContext.Authors.ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("{id:int}", Name = "GetAuthor")]
        public async Task<ActionResult<AuthorDTOWithBooks>> GetById([FromRoute] int id)
        {
            var author = await _dbContext.Authors
                .Include(x => x.BookAuthors)
                .ThenInclude(x => x.Book)
                .FirstOrDefaultAsync(x => x.Id == id);
            return _mapper.Map<AuthorDTOWithBooks>(author);
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> GetByName([FromRoute] string name)
        {
            var authors = await _dbContext.Authors.Where(dbAuthor => dbAuthor.Name.Contains(name)).ToListAsync();
            return _mapper.Map<List<AuthorDTO>>(authors);
        }

        [HttpGet("list")]
        public async Task<ActionResult<List<Author>>> GetList()
        {
            return await _dbContext.Authors.Include(x => x.Books).ToListAsync();
        }

        [HttpPost("new")]
        public async Task<ActionResult> New([FromBody] NewAuthorDTO authorDTO)
        {
            var exists = await _dbContext.Authors.AnyAsync(x => x.Name == authorDTO.Name);
            if (exists)
                return BadRequest($"Already exists an Author with name {authorDTO.Name}");

            var author = _mapper.Map<Author>(authorDTO);

            _dbContext.Add(author);
            await _dbContext.SaveChangesAsync();
            return CreatedAtRoute("GetAuthor", new { id = author.Id }, authorDTO);
        }

        [HttpPut("update/{id:int}")]
        public async Task<ActionResult> Update(NewAuthorDTO newAuthorDto, int id)
        {
            var exist = await _dbContext.Authors.AnyAsync(X => X.Id == id);

            if (!exist)
                return NotFound();

            var author = _mapper.Map<Author>(newAuthorDto);
            author.Id = id;

            _dbContext.Update(author);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await _dbContext.Authors.AnyAsync(a => a.Id == id);
            if (!existe)
            {
                return NotFound();
            }

            _dbContext.Remove(new Author { Id = id });
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}