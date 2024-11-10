using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Microsoft.AspNetCore.Mvc;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.WebAPI.Logging;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/bookauthor")]
    [ApiController]
    public class BookAuthorController : Controller
    {
        private readonly IBookAuthorsRepos _bookauthor;
        private readonly ILogger<BookAuthorController> _logger;
        public BookAuthorController(ILogger<BookAuthorController> logger, IBookAuthorsRepos bookrepos)
        {
            _bookauthor = bookrepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "BookAuthorControllerInitialization"), "BookAuthorController has been initialized.");
        }

        ////POST: api/bookauthor
        ////BODY: BookAuthor(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<BookAuthorDto>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateBookAuthorAsync([FromBody] BookAuthor BookAuthor) 
        {
            if (BookAuthor == null)
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create BookAuthor. BookAuthor data is null");
                return BadRequest("Failed to create BookAuthor. BookAuthor data is null");
            }

            try
            {
                // Создание новой записи BookAuthor
                var newBookAuthor = new BookAuthor
                {
                    BookId = BookAuthor.BookId,
                    AuthorId = BookAuthor.AuthorId
                };

                // Создание записи в базе данных
                var createdBookAuthorId = await _bookauthor.CreateBookAuthorAsync(BookAuthor);

                if (createdBookAuthorId > 0)
                {
                    BookAuthor.AuthorId = createdBookAuthorId; // Предполагается, что BookAuthor имеет свойство BookAuthorId
                    _logger.LogInformation(EventIds.Created, $"BookAuthor has been created with ID: {createdBookAuthorId}");
                    return StatusCode(201, new { Data = BookAuthor });
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Alredy exists");
                    return BadRequest("Alredy exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error. Error creating BookAuthor: {ex.Message}");
                return StatusCode(500, "Internal server error.Error creating BookAuthor"); // 500
            }
        }

        //DELETE: api/bookauthor?bookid=[]&authorid=[]
        //DELETE: api/bookauthor?authorid=[]
        //DELETE: api/bookauthor?bookid=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        public async Task<IActionResult> DeleteBookAuthorAsync([FromQuery] int? bookid, [FromQuery] int? authorid) 
        {
            try 
            {
                if (bookid.HasValue && bookid > 0 && !authorid.HasValue)
                {
                    var deleted = await _bookauthor.DeleteBookAuthorsByBookId(bookid.Value);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"BookAuthor with book ID {bookid} not found");
                        return NotFound($"BookAuthor with book ID {bookid} not found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"BookAuthor with book ID {bookid} was deleted");
                    return NoContent();
                }
                if ((authorid.HasValue && authorid > 0) && (bookid.HasValue && bookid > 0))
                {
                    var deleted = await _bookauthor.DeleteBookAuthorByBookIdAndAuthorId(bookid.Value, authorid.Value);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"BookAuthor with id Book {bookid.Value} and Author with id {authorid.Value} wasnt found");
                        return NotFound($"BookAuthor with id Book {bookid.Value} and Author with id {authorid.Value} wasnt found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $" BookAuthor with id Book {bookid.Value} and Author with id {authorid.Value} wasnt found");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to delete BookAuthor  by bookId or authorid&bookid");
                    return BadRequest("Failed to delete BookAuthor  by bookId or by authorid&bookid");
                }
            }
            catch(Exception ex)
            {

                _logger.LogError(EventIds.Error, ex, "Error deleting BookAuthor ");
                return StatusCode(500, $"Internal server error.Error deleting BookAuthor ${ex.Message}");
            }
        }
        
        //GET: api/bookauthor?bookid=[]&authorid=[]
        //GET: api/bookauthor?authorid=[]
        //GET: api/bookauthor?bookid=[]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Author>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBookAythorsAsync([FromQuery] int? bookid, [FromQuery] int? authorid) 
        {
            try 
            {
                int paramCount = (bookid.HasValue ? 1 : 0) + (authorid.HasValue ? 1 : 0);

                if (paramCount > 1 && paramCount != 0) 
                {
                    _logger.LogWarning(EventIds.Exception, "Only one query parameter ( bookid or authorid) can be used at a time.");
                    return BadRequest("Only one query parameter (bookid or authorid) can be used at a time.");
                }

                if (bookid.HasValue && bookid > 0)
                {
                    var allauthors = await _bookauthor.GetAllAuthorsByBookIdAsync(bookid.Value);    
                    if (allauthors == null || !allauthors.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No authors found for Book with ID {bookid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent authors for Book with ID {bookid}");
                    return Ok(allauthors); // 200
                }

                if (authorid.HasValue && authorid > 0)
                {
                    var allbooks = await _bookauthor.GetAllBooksByAuthorIdAsync(authorid.Value);
                    if (allbooks == null || !allbooks.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No books found with Author ID {bookid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent Books with Author ID {bookid}");
                    return Ok(allbooks); // 200
                }

                //если не указаны параметры возвращаем все книги со всеми жанрами
                var alldata = await _bookauthor.GetAllBooksWithAuthorsAsync();
                _logger.LogInformation(EventIds.Fetched, "Sent all books with genres");
                return Ok(alldata);//200
            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting BookAuthor {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


    }
}
