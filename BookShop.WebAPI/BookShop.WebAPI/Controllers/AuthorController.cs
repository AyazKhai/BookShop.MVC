using Microsoft.AspNetCore.Mvc;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.WebAPI.Logging;
using BookShop.Common.Models.Models;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using BookShop.Common.DataContext.Postgress.Repositories;
using System.Net;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/author")]
    [ApiController]
    public class AuthorController : Controller
    {
        private readonly IAuthorRepos _authorRepos;
        private readonly ILogger<AuthorController> _logger;

        public AuthorController(IAuthorRepos authorRepos, ILogger<AuthorController> logger)
        {
            _authorRepos = authorRepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "AuthorControllerInitialization"), "AuthorController has been initialized.");
        }
        [HttpGet("authorid")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAuthorByIdAync([FromQuery][Required] int authorid) 
        {
            try
            {
                if (authorid != 0 || authorid != null)
                {

                    var books = await _authorRepos.GetAuthorByIdAsync(authorid);
                    if (books == null)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No books were found for Book with id {authorid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent books for Book with id {authorid}");
                    return Ok(books); // 200
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Cant get book by id");
                    return BadRequest("Cat get book by id. Check your input data");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting BookAuthor {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
        //GET: /api/author?firstname=[]&lastname=[]
        //GET: /api/author?firstname=[]
        //GET: /api/author?lastname=[]
        //GET: api/author
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAuthorsAsync([FromQuery] string? firstname, [FromQuery] string? lastname)
        {
            try 
            {
                if (string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(lastname)) 
                {
                    var authors = await _authorRepos.GetAuthorsByFNameOrLNameParametrAsync(null, lastname);

                    if (authors == null || !authors.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Authors with LastName {lastname} wasnt found");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Authors with LastName {lastname}");
                    return Ok(authors); // 200
                }
                if (!string.IsNullOrEmpty(firstname) && string.IsNullOrEmpty(lastname))
                {
                    var authors = await _authorRepos.GetAuthorsByFNameOrLNameParametrAsync(firstname,null);

                    if (authors == null || !authors.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Authors with FirstName {firstname} wasnt found");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Authors with FirstName {firstname}");
                    return Ok(authors); // 200
                }
                if (!string.IsNullOrEmpty(firstname) && !string.IsNullOrEmpty(lastname)) 
                {
                    var authors = await _authorRepos.GetAuthorByFullNameAsync(firstname, lastname);

                    if (authors == null )
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Authors with FirstName {firstname} LastName {lastname} wasnt found");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Authors with FirstName {firstname} LastName {lastname}");
                    return Ok(authors); // 200
                }
                else
                {
                    var authors = await _authorRepos.GetAllAuthorsAsync();

                    if (authors == null)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Authors werent found");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Authors");
                    return Ok(authors); // 200
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting Author {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        ////POST: api/author
        ////BODY: Author(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateAuthorAsync([FromBody] AuthorDto newAuthor)
        {
            if (newAuthor == null)
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Author. Author data is null");
                return BadRequest("Failed to create Author. Author data is null");
            }

            try
            {
                var newAutor = new Author
                {
                    FirstName = newAuthor.FirstName,
                    LastName = newAuthor.LastName,
                    Bio = newAuthor.Bio,
                };

                var createdAuthorId = await _authorRepos.CreateAuthorAsync(newAutor);
                if (createdAuthorId > 0)
                {
                    newAutor.AuthorId = createdAuthorId;
                    _logger.LogInformation(EventIds.Created, $"Author has been created");
                    return StatusCode(201, new { data = newAutor });
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create Author");
                    return BadRequest("Failed to create Author");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating Author {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }

        //DELETE: api/author?authorid?=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAuthorAsync([FromQuery] int authorid)
        {
            if (authorid <= 0)
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Author ID.");
                return BadRequest("Invalid Author ID");
            }
            try
            {
                var deleted = await _authorRepos.DeleteAuthorAsync(authorid);
                if (!deleted)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Author with ID {authorid} not found");
                    return NotFound($"Author with ID {authorid} not found");
                }
                _logger.LogInformation(EventIds.Deleted, $"RAuthor with ID {{authorid was deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Author");
                return StatusCode(500, $"Internal server error.Error deleting Author ${ex.Message}");
            }
        }

        //PUT: api/author?authorid=[]
        //BODY: Author(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateAuthorAsync([Required][FromQuery] int author, [FromBody] Author updatedAuthor)
        {
            if (updatedAuthor == null)
            {
                _logger.LogWarning(EventIds.Exception, "AuthorID mismatch");
                return BadRequest("Author is null");
            }

            if (author != updatedAuthor.AuthorId)
            {
                _logger.LogWarning(EventIds.Exception, "Author ID mismatch");
                return BadRequest("Author ID mismatch");
            }

            try
            {
                var authorExists = await _authorRepos.GetAuthorByIdAsync(author);
                if (authorExists == null) 
                {
                    _logger.LogWarning(EventIds.NotFound, $"Author with ID {author} not found.");
                    return NotFound($"Author with ID {author} not found.");
                }

                var updated = await _authorRepos.UpdateAuthorAsync(updatedAuthor);
                if (!updated) 
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update Author.");
                    return BadRequest("Failed to update Author");
                }
                _logger.LogInformation(EventIds.Updated, $"Author with ID {author} not found.");
                return NoContent();
            }
            catch(Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, "Error updating Author");
                return StatusCode(500, $"Internal server error.Error updating Author ${ex.Message}");
            }
        }
    }
}
