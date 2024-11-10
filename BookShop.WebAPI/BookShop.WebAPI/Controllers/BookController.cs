using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Microsoft.AspNetCore.Mvc;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.WebAPI.Logging;
using System.Numerics;
using System.Net;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;
using BookShop.Common.DataContext.Postgress.Repositories;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace BookShop.WebAPI.Controllers
{
    [Route("api/books")]
    [ApiController]
    public class BookController : Controller
    {
        private readonly IBookRepos _bookrepos;
        private readonly ILogger<BookController> _logger;

        public BookController(IBookRepos bookrepos, ILogger<BookController> logger)
        {
            _bookrepos = bookrepos;
            _logger = logger;
        }

        ////POST: api/books
        ////BODY: Book(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(BookCreateDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateBookAsync(BookCreateDto newbook) 
        {
            if (newbook == null) 
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Book. Bookdata is null");
                return BadRequest("Failed to create Book. Book data is null");
            }
            try 
            {
                var newBook= new Book
                {
                    Title = newbook.Title,
                    PublisherId = newbook.PublisherId,
                    Price = newbook.Price,
                    Isbn = newbook.Isbn,
                    StockQuantity = newbook.StockQuantity,
                    BookDescription = newbook.BookDescription,
                   Imagelinks = newbook.Imagelinks
                };

                var createdBookId = await _bookrepos.CreateBookAsync(newBook);

                if (createdBookId > 0)
                {
                    newBook.BookId = createdBookId;
                    _logger.LogInformation(EventIds.Created, $"Bookhas been created with ID: {createdBookId}");
                    return StatusCode(201,createdBookId);
                }
                else 
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create Book");
                    return BadRequest("Failed to create Book");
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error. Error creating Book: {ex.Message}");
                return StatusCode(500, "Internal server error.Error creating Book"); // 500
            }
        }

        //DELETE: api/book?bookid=[]&isbn=[]&title=[]
        //DELETE: api/book?bookid=[]&isbn=[]
        //DELETE: api/book?bookid=[]&title=[]
        //DELETE: api/book?isbn=[]&title=[]
        //DELETE: api/book?title=[]
        //DELETE: api/book?isbn=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteBookAsync([FromQuery] int? bookid, [FromQuery] string? isbn, [FromQuery] string? title) 
        {

            try 
            {
                int paramcount = (bookid.HasValue ? 1 : 0) + (!string.IsNullOrEmpty(isbn) ? 1 : 0) + (!string.IsNullOrEmpty(title) ? 1 : 0);
                if (paramcount > 1)
                {
                    _logger.LogWarning(EventIds.Exception, "Only one query parameter (BookId, ISBN, or Title) can be used at a time.");
                    return BadRequest("Only one query parameter (BookId, ISBN, or Title) can be used at a time.");
                }

                if (!bookid.HasValue && string.IsNullOrEmpty(isbn) && string.IsNullOrEmpty(title))
                {
                    _logger.LogWarning(EventIds.Exception, "No query parameter provided. Either BookId, ISBN, or Title must be specified.");
                    return BadRequest("No query parameter provided. Either BookId, ISBN, or Title must be specified.");
                }

                if (!string.IsNullOrEmpty(isbn) ) 
                {
                    isbn.Trim();
                    var deleted = await _bookrepos.DeleteBookByIsbnAsync(isbn);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Book with ISBN {isbn} not found");
                        return NotFound($"Book with ISBN {isbn} not found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"Book with book ISBN {isbn} was deleted");
                    return NoContent();
                }

                if (!string.IsNullOrEmpty(title))
                {
                    title.Trim();
                    var deleted = await _bookrepos.DeleteBookByTitleAsync(title);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Book with title {title} not found");
                        return NotFound($"Book with title {title} not found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"Book with book title {title} was deleted");
                    return NoContent();
                }
                if (bookid.HasValue) 
                {
                    var deleted = await _bookrepos.DeleteBookAsync(bookid.Value);
                    if (!deleted)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Book with id {bookid} not found");
                        return NotFound($"Book with id {bookid} not found");
                    }
                    _logger.LogInformation(EventIds.Deleted, $"Book with book id {bookid} was deleted");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to delete Book  by bookId or ISBN or Title");
                    return BadRequest("Failed to delete Book  by bookId or ISBN or Title");
                }


            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Book ");
                return StatusCode(500, $"Internal server error.Error deleting Book ${ex.Message}");
            }
        }


        //GET: api/book?title=[]
        //GET: api/book/
        [HttpGet("bybookid")]
        [ProducesResponseType(200, Type = typeof(Book))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBookByIdAync([FromQuery][Required] int? bookid)
        {
            try
            {
                if (bookid != 0 || bookid != null)
                {

                    var books = await _bookrepos.GetAllBooksByIdAsync(bookid);
                    if (books == null)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No books were found for Book with id {bookid}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent books for Book with id {bookid}");
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

        //GET: api/book?title=[]
        //GET: api/book/
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllBooksAync(string? title) 
        {
            try
            {
                if (!string.IsNullOrEmpty(title))
                {
                    title.Trim();
                    var books = await _bookrepos.GetAllBooksByTitleAsync(title);
                    if (books == null || !books.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No books were found for Book with Title {title}");
                        return NotFound();
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent books for Book with Title {title}");
                    return Ok(books); // 200
                }
                else 
                {
                    var alldata = await _bookrepos.GetAllBoksAsync();
                    _logger.LogInformation(EventIds.Fetched, "Sent all books");
                    return Ok(alldata);//200
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting BookAuthor {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        // GET: api/book/array
        [HttpGet("array")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetBooksArrayAsync(int? limit, int? offset, string? sort, string? order, int? genreid)
        {
            try
            {
                // Проверка входных данных
                var validationResult = ValidateInput(limit, offset, sort, order, genreid);
                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Неверные параметры: {validationResult.ErrorMessage}");
                    return BadRequest(validationResult.ErrorMessage);
                }

                // Вызов репозитория для получения книг с параметрами фильтрации и сортировки
                var books = await _bookrepos.GetBooksArrayAsync(limit ?? 10, offset ?? 0, sort ?? "title", order ?? "asc", genreid);

                if (books == null || !books.Any())
                {
                    return NotFound("Книги не найдены.");
                }

                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Внутренняя ошибка сервера. Ошибка при получении книг: {ex.Message}");
                return StatusCode(500, "Внутренняя ошибка сервера. Попробуйте позже.");
            }
        }


        //PUT: api/boook?id=[]
        //BODY: Book(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateBookAsync([FromQuery][Required]int id,[FromBody] BookUpdateDto newBook) 
        {
            if (newBook == null) 
            {
                _logger.LogWarning(EventIds.Exception, "Book is null");
                return BadRequest("Book is null");
            }

            if (id != newBook.BookId)
            {
                _logger.LogWarning(EventIds.Exception, "Book ID mismatch");
                return BadRequest("Book ID mismatch");
            }

            try
            {
                var bookExists = await _bookrepos.GetAllBooksByIdAsync(id);
                if (bookExists == null) 
                {
                    _logger.LogWarning(EventIds.NotFound, $"Book with ID {id} not found.");
                    return NotFound($"Book with ID {id} not found.");
                }

                // Валидация ISBN
                newBook.Isbn = newBook.Isbn?.Trim();
                if (!string.IsNullOrEmpty(newBook.Isbn) && newBook.Isbn.Trim().Length != 13)
                {
                    _logger.LogWarning(EventIds.Exception, "ISBN must be 13 characters.");
                    return BadRequest("ISBN must be 13 characters.");
                }

                // Обновление книги
                var updated = await _bookrepos.UpdateBookAsync(newBook);
                if (!updated)
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update Book.");
                    return BadRequest("Failed to update Book.");
                }

                _logger.LogInformation(EventIds.Updated, $"Book with ID {id} was successfully updated.");
                return Ok(new { Message = "Book updated successfully." });
            }
            catch (Exception ex) 
            {
                // Лог ошибки
                _logger.LogError(EventIds.Error, ex, $"Error occurred while updating Book with ID {id}: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private (bool IsValid, string ErrorMessage) ValidateInput(int? limit, int? offset, string? sort, string? order, int? genreid)
        {
            // Проверка предела и смещения (должны быть положительными)
            if (limit.HasValue && limit <= 0)
            {
                return (false, "Параметр 'limit' должен быть положительным числом.");
            }
            if (offset.HasValue && offset < 0)
            {
                return (false, "Параметр 'offset' не может быть отрицательным.");
            }

            // Проверка параметра сортировки
            var allowedSortFields = new[] { "title", "price", "stockquantity" };
            if (!string.IsNullOrEmpty(sort) && !allowedSortFields.Contains(sort.ToLower()))
            {
                return (false, $"Параметр 'sort' может быть только одним из следующих: {string.Join(", ", allowedSortFields)}.");
            }

            // Проверка порядка сортировки (должен быть asc или desc)
            if (!string.IsNullOrEmpty(order) && !(order.Equals("asc", StringComparison.OrdinalIgnoreCase) || order.Equals("desc", StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Параметр 'order' может быть только 'asc' или 'desc'.");
            }

            // Проверка genreid (если это нужно, например, должно быть положительным или null)
            if (genreid.HasValue && genreid <= 0)
            {
                return (false, "Параметр 'genreid' должен быть положительным числом.");
            }

            // Все проверки пройдены
            return (true, string.Empty);
        }

    }
}
