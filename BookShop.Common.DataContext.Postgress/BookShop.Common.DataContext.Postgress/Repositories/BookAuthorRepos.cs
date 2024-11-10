using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class BookAuthorRepos:IBookAuthorsRepos
    {
        DapperDbContext _context;
        public BookAuthorRepos(DapperDbContext context) 
        {
            _context = context;
        }

        public async Task<int> CreateBookAuthorAsync(BookAuthor author)
        {
            if (author == null) 
            {
                throw new ArgumentNullException(nameof(author), "Can not be null");
            }
            try 
            {
                var checkquery = "SELECT COUNT(1) FROM BookAuthors WHERE BookId = @BookId AND AuthorID = @AuthorID";
                var insertQuery = "INSERT INTO BookAuthors(BookId,AuthorId) " +
                    "Values (@BookId,@AuthorId) RETURNING BookAuthorId";

                using(var connection = _context.CreateConnection()) 
                {
                    var exists = await connection.ExecuteScalarAsync<bool>(checkquery, new
                    {
                        BookId = author.BookId,
                        AuthorId = author.AuthorId
                    });

                    if (exists)
                    {
                        //throw new InvalidOperationException("The combination of BookId and AuthorId already exists.");
                        return 0;
                    }
                    else 
                    {
                        var newBookAuthorId = await connection.ExecuteScalarAsync<int>(insertQuery, new
                        {
                            BookId = author.BookId,
                            AuthorId = author.AuthorId
                        });
                        return newBookAuthorId;
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка CreateBookAuthorAsync {ex.Message}");
                throw;
            }
        }

        
        public async Task<bool> DeleteBookAuthorsByBookId(int bookid)
        {
            try
            {

                if (bookid != null && bookid > 0)
                {
                    var query = "DELETE FROM BookAuthors WHERE BookId = @BookId ";
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
                Console.Error.WriteLine($"Ошибка DeleteBookAuthorsByBookId {ex.Message}");
                throw ex;
            }
        }
        public async Task<bool> DeleteBookAuthorByBookIdAndAuthorId(int bookid, int authorid)
        {
            try
            {
                if (bookid > 0 && authorid > 0) 
                {
                    var query = "Delete From BookAuthors " +
                        "WHERE BookId = @ BookId AND AuthorId = @AuthorId";
                    using (var connection = _context.CreateConnection()) 
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { BookId = bookid, AuthorId = authorid });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteBookAuthorByBookIdAndAuthorId: {ex.Message}");
                throw;
            }
        }



        public async Task<IEnumerable<Author>> GetAllAuthorsByBookIdAsync(int bookid)
        {
            try
            {
                if (bookid != null && bookid > 0)
                {
                    var query = @"SELECT au.* 
                          FROM Authors au
                          JOIN BookAuthors ba ON au.AuthorId = ba.AuthorId 
                          WHERE ba.BookId = @BookId";

                    using (var connection = _context.CreateConnection())
                    {
                        return await connection.QueryAsync<Author>(query, new { BookId = bookid });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении авторов для книги по id: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении авторов для книги по id: {ex}");
            }
        }

        public async Task<IEnumerable<Book>> GetAllBooksByAuthorIdAsync(int authorid)
        {
            try
            {
                var query = @"SELECT b.BookId, b.Title, b.Price, b.ISBN, b.StockQuantity, b.BookDescription, b.ImageLinks, 
                      a.AuthorId, a.FirstName, a.LastName, a.Bio
               FROM Books b 
               LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
               LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId
               WHERE a.AuthorId = @AuthorId"; // Добавляем условие для фильтрации по AuthorId

                var bookDictionary = new Dictionary<int, Book>();

                using (var connection = _context.CreateConnection())
                {
                    var books = await connection.QueryAsync<Book, Author, Book>(
                        query,
                        (book, author) =>
                        {
                            if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                            {
                                currentBook = book;
                                currentBook.Authors = new List<Author>();
                                bookDictionary.Add(currentBook.BookId, currentBook);
                            }

                            // Добавляем автора только если он существует
                            if (author != null)
                            {
                                currentBook.Authors.Add(author);
                            }

                            return currentBook;
                        },
                        new { AuthorId = authorid }, // Передаем параметр AuthorId
                        splitOn: "AuthorId"); // Указываем, что разделение сущностей происходит по "AuthorId"

                    return bookDictionary.Values.ToList(); // Возвращаем список книг
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении книг с авторами: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении книг с авторами: {ex}");
            }
            //try
            //{
            //    if (authorid > 0)
            //    {
            //        //var query = @"SELECT b.* 
            //        //      FROM Books b 
            //        //      JOIN BookAuthors ba ON b.BookId = ba.BookId 
            //        //      WHERE ba.AuthorId = @AuthorId";

            //        var query = @"SELECT b.BookId, b.Title, b.Price, b.ISBN, b.StockQuantity, b.bookDescription, b.ImageLinks, 
            //                 a.AuthorId, a.FirstName, a.LastName, a.Bio
            //          FROM Books b 
            //          LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
            //          LEFT JOIN Authors a ON ba.AuthorId = a.@AuthorId";

            //        using (var connection = _context.CreateConnection())
            //        {
            //            return await connection.QueryAsync<Book>(query, new { AuthorId = authorid });
            //        }
            //    }
            //    return null;
            //}
            //catch (Exception ex)
            //{
            //    Console.Error.WriteLine($"Ошибка при получении книг по AuthorId: {ex.Message}");
            //    throw new ApplicationException($"Ошибка при получении книг по AuthorId: {ex}");
            //}
        }

        public async Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync()
        {
            try
            {
                var query = @"SELECT b.BookId, b.Title, b.Price, b.ISBN, b.StockQuantity, b.bookDescription, b.ImageLinks, 
                             a.AuthorId, a.FirstName, a.LastName, a.Bio
                      FROM Books b 
                      LEFT JOIN BookAuthors ba ON b.BookId = ba.BookId
                      LEFT JOIN Authors a ON ba.AuthorId = a.AuthorId";

                var bookDictionary = new Dictionary<int, Book>();

                using (var connection = _context.CreateConnection())
                {
                    var books = await connection.QueryAsync<Book, Author, Book>(
                        query,
                        (book, author) =>
                        {
                            if (!bookDictionary.TryGetValue(book.BookId, out var currentBook))
                            {
                                currentBook = book;
                                currentBook.Authors = new List<Author>();
                                bookDictionary.Add(currentBook.BookId, currentBook);
                            }

                            // Добавляем автора только если он существует
                            if (author != null)
                            {
                                currentBook.Authors.Add(author);
                            }

                            return currentBook;
                        },
                        splitOn: "AuthorId");  // Важно указывать только "AuthorId" для разделения сущностей

                    return books.Distinct().ToList();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении книг с авторами: {ex.Message}");
                throw new ApplicationException($"Ошибка при получении книг с авторами: {ex}");
            }
        }

        public async Task<IEnumerable<BookAuthor>> GetAllBookAuthorsAsync()
        {
            try
            {
                var query = "Select * From BookAuthors";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<BookAuthor>(query);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении BookAuthors {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка BookAuthors {ex}");
            }
        }
    }
}
