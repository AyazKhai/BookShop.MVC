using BookShop.Common.DataContext.Postgress.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.Common.Models.Models;
using BookShop.WebAPI.Logging;
using BookShop.Common.DataContext.Postgress.Repositories;
using System.Net;
using System.ComponentModel.DataAnnotations;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/bookgenre")]
    [ApiController]
    public class BookGenresController : Controller
    {
        private readonly IBookGenresRepos _bookgenres;
        private readonly ILogger<BookGenresController> _logger;

        public BookGenresController(IBookGenresRepos bookgenres, ILogger<BookGenresController> logger)
        {
            _bookgenres = bookgenres;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "BookGenresControllerInitialization"), "BookGenresController has been initialized.");
        }
        ////POST: api/bookgenre
        ////BODY: BookGenre(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<BookGenreDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateBookGenreAsync([FromBody] BookGenreDto BookGenre) 
        {
            if(BookGenre == null) 
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create BookGenre. BookGenre data is null");
                return BadRequest("Failed to create BookGenre. BookGenre data is null");
            }
            try 
            {
                var newBookGenre = new BookGenre 
                {
                    BookId = BookGenre.BookId,
                    GenreId = BookGenre.GenreId,
                };

                var createdBookGenreId = await _bookgenres.CreateBookGenreAsync(newBookGenre);
                if (createdBookGenreId > 0)
                {
                    newBookGenre.BookgenreId = createdBookGenreId;
                    _logger.LogInformation(EventIds.Created, $"BookGenre has been created ");
                    return StatusCode(201, new { Data = newBookGenre });
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create BookGenre");
                    return BadRequest("Failed to create BookGenre");
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating BookGenre {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }

        //DELETE: api/bookgenre?bookid=[]&genreid=[]
        //DELETE: api/bookgenre?genreid=[]
        //DELETE: api/bookgenre?bookid=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        public async Task<IActionResult> DeleteBookGenreByIdAsync([FromQuery] int? bookid, [FromQuery] int? genreid)
        {
            try
            {
                if(bookid.HasValue && bookid > 0 && !genreid.HasValue)
                {
                    var deleted = await _bookgenres.DeleteBookGenresByBookIdAsync(bookid.Value);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"BookGenre with book ID {bookid} not found");
                        return NotFound($"BookGenre with ID book {bookid} not found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"BookGenre with ID book {bookid} was deleted");
                    return NoContent();
                }

                if ((genreid.HasValue && genreid > 0) && (bookid.HasValue && bookid > 0))
                {
                    var deleted = await _bookgenres.DeleteBookGenresByGenreIdAndBookIdAsync(genreid.Value, bookid.Value);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"BookGenre with id {bookid.Value} and Genre with id {genreid.Value} wasnt found");
                        return NotFound($"BookGenre with id {bookid.Value} and Genre with id {genreid.Value} wasnt found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"BookGenre with id {bookid.Value} and Genre with id {genreid.Value} was succesfully deleted");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to delete BookGenre by bookId or genreid");
                    return BadRequest("Failed to delete BookGenre by bookId or genreid");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting BookGenre");
                return StatusCode(500, $"Internal server error.Error deleting BookGenre${ex.Message}");
            }
        }

        //GET: api/bookgenre
        //GET: api/bookgenre?bookid=[]&genreid=[]
        //GET: api/bookgenre?genreid=[]
        //GET: api/bookgenre?bookid=[]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Genre>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBookgenresAsync([FromQuery] int? bookid, [FromQuery] int? genreid) 
        {
            try
            {
                // Проверка, что передан только один параметр
                int paramCount = (bookid.HasValue ? 1 : 0) + (genreid.HasValue ? 1 : 0);

                if (paramCount > 1 && paramCount != 0)
                {
                    _logger.LogWarning(EventIds.Exception, "Only one query parameter ( bookid or genreid) can be used at a time.");
                    return BadRequest("Only one query parameter (bookid or genreid) can be used at a time.");
                }


                if(bookid.HasValue && bookid > 0) 
                {
                    var allgenres = await _bookgenres.GetAllGenreByBookIdAsync(bookid.Value);
                    if (allgenres == null || !allgenres.Any()) 
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No genres found for Book with ID {bookid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent genres for Book with ID {bookid}");
                    return Ok(allgenres); // 200
                }

                if (genreid.HasValue && genreid > 0) 
                {
                    var allbooks = await _bookgenres.GetAllBooksByGenreIdAsync(genreid.Value);
                    if (allbooks == null || !allbooks.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No books found with Genre ID {bookid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Books with Genre ID {bookid}");
                    return Ok(allbooks); // 200
                }

                //если не указаны параметры возвращаем все книги со всеми жанрами
                var alldata = await _bookgenres.GetAllBooksWithAllGenresAsync();
                _logger.LogInformation(EventIds.Fetched, "Sent all books with genres");
                return Ok(alldata);//200

            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting BookGenre {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


    }
}