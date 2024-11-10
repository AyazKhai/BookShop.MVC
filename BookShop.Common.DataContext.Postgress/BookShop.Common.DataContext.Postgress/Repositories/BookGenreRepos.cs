using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class BookGenreRepos : IBookGenresRepos
    {
        DapperDbContext _context;
        public BookGenreRepos(DapperDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateBookGenreAsync(BookGenre bookGenre)
        {
            if (bookGenre == null)
            {
                throw new ArgumentNullException(nameof(bookGenre), "Can not be null");
            }
            try
            {
                var checkQuery = "SELECT COUNT(1) FROM BookGenres WHERE BookId = @BookId AND GenreID = @GenreID";
                var insertQuery = "INSERT INTO BookGenres(BookId, GenreID) VALUES(@BookId, @GenreID) RETURNING BookGenreID";

                using (var connection = _context.CreateConnection())
                {
                    // Проверяем, существует ли такая запись
                    var exists = await connection.ExecuteScalarAsync<bool>(checkQuery, new
                    {
                        BookId = bookGenre.BookId,
                        GenreID = bookGenre.GenreId
                    });

                    if (exists)
                    {
                        return 0;
                        // Если запись существует, возвращаем идентификатор существующей записи
                        //throw new InvalidOperationException("The combination of BookId and GenreId already exists.");
                    }
                    else
                    {
                        // Если не существует, выполняем вставку
                        var newBookGenreId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                        {
                            BookId = bookGenre.BookId,
                            GenreID = bookGenre.GenreId
                        });
                        return newBookGenreId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка CreateBookGenreAsync {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteBookGenresByBookIdAsync(int bookid)
        {
            try
            {
                if (bookid != null && bookid > 0) 
                {
                    var query = "DELETE FROM BookGenres WHERE BookId = @BookId ";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { BookId = bookid });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookGenresByBookIdAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteBookGenresByGenreIdAndBookIdAsync(int genreid, int bookid)
        {
            try
            {
                if (bookid > 0 && genreid > 0)
                {
                    var query = "DELETE FROM BookGenres WHERE BookId = @BookId AND GenreId = @GenreId";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { BookId = bookid, GenreId = genreid });
                        return affectedRows > 0;
                    }
                }
                else 
                {
                    Console.Error.WriteLine($"unvaluable parametrs");
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookGenresByGenreIdAndBookIdAsync: {ex.Message}");
                throw;
            }
        }

        //не используется
        public async Task<IEnumerable<BookGenre>> GetAllBookgenresAsync()
        {
            try 
            {
                var query = "Select * From BookGenres";

                using (var connection = _context.CreateConnection()) 
                {
                    return await connection.QueryAsync<BookGenre>( query );
                } 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении Bookgenres {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Bookgenres {ex}");
            }
        }

        //new version
        public async Task<IEnumerable<Book>> GetAllBooksByGenreIdAsync(int genreid)
        {
            try
            {
                var query = @"SELECT b.BookId, b.Title, b.PublisherId, b.Price, b.ISBN, b.StockQuantity, b.BookDescription, b.ImageLinks::jsonb,
                             a.AuthorId, a.FirstName, a.LastName, a.Bio,
                             g.GenreId, g.GenresName, 
                             r.ReviewId, r.CommentText, r.Rating,
                             p.PublisherId, p.PublisherName
                      FROM Books b
                      LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                      LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
                      LEFT JOIN BookGenres bg ON b.BookId = bg.BookId
                      LEFT JOIN Genres g ON bg.GenreId = g.GenreId
                      LEFT JOIN Reviews r ON b.BookId = r.BookId
                      LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId
                      WHERE g.GenreId = @GenreId";

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
                        new { GenreId = genreid }, // Параметры запроса
                        splitOn: "AuthorId,GenreId,ReviewId,PublisherId" // Указываем, где происходит разделение сущностей
                    );

                    return bookDictionary.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Books: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Books: {ex}");
            }
        }
        //old version
        //public async Task<IEnumerable<Book>> GetAllBooksByGenreIdAsync(int genreid)
        //{
        //    try
        //    {
        //        if (genreid != null && genreid >0) 
        //        {
        //            var query = "SELECT b.BookId, b.Title, b.PublisherId, b.Price, b.ISBN, b.StockQuantity, b.BookDescription, b.ImageLinks " +
        //                "FROM Books b JOIN BookGenres bg ON b.BookId = bg.BookId  " +
        //                "WHERE bg.GenreId = @GenreId";

        //            using (var connection = _context.CreateConnection()) 
        //            {
        //                return await connection.QueryAsync<Book>(query, new {GenreId = genreid });
        //            }
        //        }
        //        return null;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.Error.WriteLine($"Ошибка при получении книг из таблицы BookGenre по Genre id: {ex}");
        //        throw new ApplicationException("Ошибка при получении книг из таблицы BookGenre по Genre id.");
        //    }
        //}

        public async Task<IEnumerable<Genre>> GetAllGenreByBookIdAsync(int bookid)
        {
            try
            {
                if (bookid != null && bookid > 0)
                {
                    var query = "SELECT g.* FROM Genres g " +
                        "JOIN BookGenres bg ON g.GenreId = bg.GenreId " +
                        "WHERE bg.BookId = @BookId";

                    using (var connection = _context.CreateConnection())
                    {
                        return await connection.QueryAsync<Genre>(query, new { BookId = bookid });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении жанров для книги по id {ex.Message}");
                throw new ApplicationException("Ошибка при получении книгжанров для книги по id");
            }
        }
       


        //все книги с жанрами сразу
        public async Task<IEnumerable<Book>> GetAllBooksWithAllGenresAsync()
        {
            try
            {
                var query = @"SELECT b.BookId, b.Title, b.PublisherId, b.Price, b.ISBN, b.StockQuantity, b.BookDescription, b.ImageLinks::jsonb,
                             a.AuthorId, a.FirstName, a.LastName, a.Bio,
                             g.GenreId, g.GenresName, 
                             r.ReviewId, r.CommentText, r.Rating,
                             p.PublisherId, p.PublisherName
                      FROM Books b
                      LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                      LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
                      LEFT JOIN BookGenres bg ON b.BookId = bg.BookId
                      LEFT JOIN Genres g ON bg.GenreId = g.GenreId
                      LEFT JOIN Reviews r ON b.BookId = r.BookId
                      LEFT JOIN Publishers p ON b.PublisherId = p.PublisherId";

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
                        }, // Параметры запроса
                        splitOn: "AuthorId,GenreId,ReviewId,PublisherId" // Указываем, где происходит разделение сущностей
                    );

                    return bookDictionary.Values.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Books: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Books: {ex}");
            }
        }


    }
}
