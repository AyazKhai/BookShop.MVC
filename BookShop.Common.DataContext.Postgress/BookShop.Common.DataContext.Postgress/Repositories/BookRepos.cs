using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;
using BookShop.Common.Models.Postgress.Models;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class BookRepos : IBookRepos
    {
        DapperDbContext _context;
        public BookRepos(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateBookAsync(Book book)
        {
            if (book == null) 
            {
                throw new ArgumentNullException(nameof(Author), "Author cannot be null");
            }
            try 
            {
                var query = "INSERT INTO Books(title,PublisherId,Price,ISBN,StockQuantity,BookDescription,ImageLinks) " +
                    "Values (@title,@PublisherId,@Price,@ISBN,@StockQuantity,@BookDescription,@ImageLinks::jsonb) " +
                    "RETURNING BookId";

                using(var connection = _context.CreateConnection()) 
                {
                    var newBookId = await connection.ExecuteScalarAsync<int>(query, new 
                    {
                        Title = book.Title,
                        PublisherId = book.PublisherId,
                        Price = book.Price,
                        Isbn = book.Isbn,
                        StockQuantity = book.StockQuantity,
                        BookDescription = book.BookDescription,
                        ImageLinks = book.Imagelinks

                    });
                    return newBookId;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка CreateBookAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    var query = "DELETE From Books WHERE BookId = @BookId";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { BookId = id });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteBookByIsbnAsync(string isbn)
        {
            try
            {
                isbn.Trim();
                if (isbn != null && !string.IsNullOrEmpty(isbn))
                {
                    var query = "DELETE From Books WHERE Isbn = @Isbn";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { Isbn = isbn });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookByIsbnAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteBookByTitleAsync(string title)
        {
            try
            {
                title.Trim();
                if (title != null && !string.IsNullOrEmpty(title))
                {
                    var query = "DELETE From Books WHERE Title = @Title";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { Title = title });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookByTitleAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<IEnumerable<Book>> GetAllBoksAsync()
        {
            try
            {
               
               // 3
                var query = @"SELECT b.*, a.AuthorId, a.FirstName, a.LastName, a.Bio, 
                                 g.GenreId, g.GenresName, 
                                 r.ReviewId, r.CommentText, r.Rating, p.PublisherId, p.PublisherName, b.ImageLinks::jsonb
                          FROM Books b 
                          LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId 
                          LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId 
                          LEFT JOIN BookGenres bg ON b.BookId = bg.BookId 
                          LEFT JOIN Genres g ON bg.GenreId = g.GenreId 
                          LEFT JOIN Reviews r ON b.BookId = r.BookId "+
                          "LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId";

                //var query = @"SELECT b.*, a.AuthorId, a.FirstName, a.LastName, a.Bio, g.GenreId, g.GenresName, r.ReviewId, r.CommentText, r.Rating " +
                //        "FROM Books b " +
                //        "LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId " +
                //        "LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId " +
                //        "LEFT JOIN BookGenres bg ON b.BookId = bg.BookId " +
                //        "LEFT JOIN Genres g ON bg.GenreId = g.GenreId " +
                //        "LEFT JOIN Reviews r ON b.BookId = r.BookId";
                //{ "imageUri": "Marble-with-model-copie.jpg"}
                using (var connection = _context.CreateConnection())
                {
                    var bookDictionary = new Dictionary<int, Book>();

                    var books = await connection.QueryAsync<Book, Author, Genre, Review, Publisher, Book>(
                            query,
                            (book, author, genre, review, publisher) =>
                            {
                                if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                                {
                                    {


                                        currentBook = book;
                                        currentBook.Authors = new List<Author>();
                                        currentBook.Bookgenres = new List<Genre>();
                                        currentBook.Reviews = new List<Review>();
                                        currentBook.Publisher = publisher;


                                        bookDictionary.Add(currentBook.BookId, currentBook);
                                    }
                                }

                                // Добавляем авторов, жанры и рецензии (по аналогии с вашим кодом)
                                if (author != null && !currentBook.Authors.Any(a => a.AuthorId == author.AuthorId))
                                {
                                    currentBook.Authors.Add(author);
                                }
                                if (genre != null && !currentBook.Bookgenres.Any(g => g.GenreId == genre.GenreId))
                                {
                                    currentBook.Bookgenres.Add(genre);
                                }
                                if (review != null && !currentBook.Reviews.Any(r => r.ReviewId == review.ReviewId))
                                {
                                    currentBook.Reviews.Add(review);
                                }

                                return currentBook;
                            },
                            splitOn: "AuthorId, GenreId, ReviewId, PublisherId" // Указываем, где происходит разделение сущностей // Параметры запроса
                        ); // Указываем, где происходит разделение сущностей
                    
                    return bookDictionary.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Books: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Books: {ex}");
            }
        }

        public async Task<IEnumerable<Book>> GetAllBooksByTitleAsync(string title)
        {
            try
            {
                if (!string.IsNullOrEmpty(title)) 
                {

                    //var query = "SELECT b.*, a.AuthorId, a.FirstName, a.LastName, a.Bio, g.GenreId, g.GenresName, r.ReviewId, " + 
                    //    "r.CommentText, r.Rating FROM Books b " +
                    //    "LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId " +
                    //    "LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId LEFT JOIN BookGenres bg ON b.BookId = bg.BookId " +
                    //    "LEFT JOIN Genres g ON bg.GenreId = g.GenreId " +
                    //    "LEFT JOIN Reviews r ON b.BookId = r.BookId " +
                    //    $"WHERE b.Title = '{title}'";

                    var query = @"SELECT b.*, a.AuthorId, a.FirstName, a.LastName, a.Bio, g.GenreId, g.GenresName, r.ReviewId, r.CommentText, r.Rating, p.PublisherId, p.PublisherName " +
                                  "FROM Books b " +
                                  "LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId " +
                                  "LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId " +
                                  "LEFT JOIN BookGenres bg ON b.BookId = bg.BookId " +
                                  "LEFT JOIN Genres g ON bg.GenreId = g.GenreId " +
                                  "LEFT JOIN Reviews r ON b.BookId = r.BookId " +
                                  "LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId " + // Добавлено соединение с таблицей Publishers
                                  $"WHERE b.Title = '{title}'";


                    
                    using (var connection = _context.CreateConnection())
                    {
                        var bookDictionary = new Dictionary<int, Book>();

                        var books = await connection.QueryAsync<Book, Author, Genre, Review,Publisher, Book>(
                                query,
                                (book, author, genre, review, publisher) =>
                                {
                                    if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                                    {
                                        currentBook = book;
                                        currentBook.Authors = new List<Author>();
                                        currentBook.Bookgenres = new List<Genre>();
                                        currentBook.Reviews = new List<Review>();
                                        currentBook.Publisher = publisher; // Добавляем издателя
                                        bookDictionary.Add(currentBook.BookId, currentBook);
                                    }

                                    // Добавляем авторов, жанры и рецензии (по аналогии с вашим кодом)
                                    if (author != null && !currentBook.Authors.Any(a => a.AuthorId == author.AuthorId))
                                    {
                                        currentBook.Authors.Add(author);
                                    }
                                    if (genre != null && !currentBook.Bookgenres.Any(g => g.GenreId == genre.GenreId))
                                    {
                                        currentBook.Bookgenres.Add(genre);
                                    }
                                    if (review != null && !currentBook.Reviews.Any(r => r.ReviewId == review.ReviewId))
                                    {
                                        currentBook.Reviews.Add(review);
                                    }

                                    return currentBook;
                                },
                                splitOn: "AuthorId, GenreId, ReviewId, PublisherId", // Указываем, где происходит разделение сущностей
                                param: new { Title = title } // Параметры запроса
                            ); // Указываем, где происходит разделение сущностей

                        return bookDictionary.Values.ToList();
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Books {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Books {ex}");
            }
        }
        public async Task<IEnumerable<Book>> GetBooksArrayAsync(int limit = 100, int offset = 0, string? sort = null, string? order = null, int? genreid = null)
        {
            try
            {
                // Сортировка по умолчанию, если параметры сортировки не указаны
                sort = !string.IsNullOrEmpty(sort) ? sort.ToLower() : "title";
                order = !string.IsNullOrEmpty(order) && order.ToLower() == "desc" ? "DESC" : "ASC";

                // Формируем основной запрос для книг с сортировкой и пагинацией
                var query = $@"
WITH FilteredBooks AS (
    SELECT b.BookId
    FROM Books b
    {(genreid.HasValue ? "JOIN BookGenres bg ON b.BookId = bg.BookId" : "")}
    {(genreid.HasValue ? "WHERE bg.GenreId = @GenreId" : "")}
    ORDER BY 
        {(sort == "title" ? "b.Title" : sort == "price" ? "b.Price" : "b.StockQuantity")} {order},
        b.BookId ASC
    LIMIT @Limit OFFSET @Offset
)
SELECT b.*, 
       a.AuthorId, a.FirstName, a.LastName, a.Bio, 
       g.GenreId, g.GenresName, 
       r.ReviewId, r.CommentText, r.Rating, 
       p.PublisherId, p.PublisherName
FROM FilteredBooks fb
JOIN Books b ON fb.BookId = b.BookId
LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
LEFT JOIN BookGenres bg ON b.BookId = bg.BookId
LEFT JOIN Genres g ON bg.GenreId = g.GenreId
LEFT JOIN Reviews r ON b.BookId = r.BookId
LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId
ORDER BY 
    {(sort == "title" ? "b.Title" : sort == "price" ? "b.Price" : "b.StockQuantity")} {order},
    b.BookId ASC;
";

                using (var connection = _context.CreateConnection())
                {
                    var bookDictionary = new Dictionary<int, Book>();

                    var books = await connection.QueryAsync<Book, Author, Genre, Review, Publisher, Book>(
                        query,
                        (book, author, genre, review, publisher) =>
                        {
                            if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                            {
                                currentBook = book;
                                currentBook.Authors = new List<Author>();
                                currentBook.Bookgenres = new List<Genre>();
                                currentBook.Reviews = new List<Review>();
                                currentBook.Publisher = publisher;
                                bookDictionary.Add(currentBook.BookId, currentBook);
                            }

                            if (author != null && !currentBook.Authors.Any(a => a.AuthorId == author.AuthorId))
                            {
                                currentBook.Authors.Add(author);
                            }
                            if (genre != null && !currentBook.Bookgenres.Any(g => g.GenreId == genre.GenreId))
                            {
                                currentBook.Bookgenres.Add(genre);
                            }
                            if (review != null && !currentBook.Reviews.Any(r => r.ReviewId == review.ReviewId))
                            {
                                currentBook.Reviews.Add(review);
                            }

                            return currentBook;
                        },
                        splitOn: "AuthorId, GenreId, ReviewId, PublisherId",
                        param: new { Limit = limit, Offset = offset, GenreId = genreid }
                    );

                    return bookDictionary.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Books {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Books {ex}");
            }
        }


        public async Task<Book?> GetAllBooksByIdAsync(int? id)
        {
            try
            {
                if (id.HasValue && id.Value > 0)
                {
                    var query = @"SELECT b.*, a.AuthorId, a.FirstName, a.LastName, a.Bio, 
                                 g.GenreId, g.GenresName, 
                                 r.ReviewId, r.CommentText, r.Rating 
                          FROM Books b 
                          LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId 
                          LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId 
                          LEFT JOIN BookGenres bg ON b.BookId = bg.BookId 
                          LEFT JOIN Genres g ON bg.GenreId = g.GenreId 
                          LEFT JOIN Reviews r ON b.BookId = r.BookId 
                          WHERE b.BookId = @BookId";

                    using (var connection = _context.CreateConnection())
                    {
                        var bookDictionary = new Dictionary<int, Book>();

                        var books = await connection.QueryAsync<Book, Author, Genre, Review, Book>(
                            query,
                            (book, author, genre, review) =>
                            {
                                if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                                {
                                   
                                    currentBook = book;
                                    currentBook.Authors = new List<Author>();
                                    currentBook.Bookgenres = new List<Genre>();
                                    currentBook.Reviews = new List<Review>();
                                    bookDictionary.Add(currentBook.BookId, currentBook);
                                    // Десериализуем ImageLinks (если он не null)
                                    // Преобразуем ImageLinks вручную
                                   

                                }

                                // Добавляем автора
                                if (author != null && !currentBook.Authors.Any(a => a.AuthorId == author.AuthorId))
                                {
                                    currentBook.Authors.Add(author);
                                }

                                // Добавляем жанр
                                if (genre != null && !currentBook.Bookgenres.Any(g => g.GenreId == genre.GenreId))
                                {
                                    currentBook.Bookgenres.Add(genre);
                                }

                                // Добавляем рецензию
                                if (review != null && !currentBook.Reviews.Any(r => r.ReviewId == review.ReviewId))
                                {
                                    currentBook.Reviews.Add(review);
                                }

                                return currentBook;
                            },
                            param: new { BookId = id.Value },
                            splitOn: "AuthorId, GenreId, ReviewId");

                        return bookDictionary.Values.FirstOrDefault();  // Возвращаем одну книгу или null
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении книги: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении книги: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateBookAsync(BookUpdateDto book)
        {
            try 
            {
                book.Isbn.Trim();
                if (!string.IsNullOrEmpty(book.Isbn) && book.Isbn.Length != 13) 
                {
                    Console.Error.WriteLine($"Ошибка UpdateBookAsync длина ISBN не соответствует 13");
                    return false;
                }
                var query = "UPDATE Books " +
                    "Set Title = COALESCE(@Title,Title), " +
                    "Price = COALESCE(@Price,Price), " +
                    "ISBN = COALESCE(@ISBN,ISBN), " +
                    "StockQuantity = COALESCE(@StockQuantity,StockQuantity), " +
                    "BookDescription = COALESCE(@BookDescription,BookDescription), " +
                    "ImageLinks = COALESCE(@ImageLinks::json,ImageLinks) " +
                    "Where BookId = @BookId";

                using (var connection = _context.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(query,book);
                    return affectedRows > 0;
                }
            }
            catch(Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка UpdateBookAsync {ex.Message}");
                return false;
            }
        }

        public Task<IEnumerable<Book>> GetAllBoksAsync(int limit, int offset, string sort, string order)
        {
            throw new NotImplementedException();
        }
    }
}
