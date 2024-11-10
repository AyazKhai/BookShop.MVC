using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class AuthorRepos : IAuthorRepos
    {
        DapperDbContext _context;
        public AuthorRepos(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAuthorAsync(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(Author), "Author cannot be null");
            }
            try
            {
                var query = "INSERT INTO Authors(FirstName,LastName,Bio) " +
                    "Values(@FirstName,@LastName,@Bio) " +
                    "RETURNING AuthorId";

                using (var connection = _context.CreateConnection())
                {
                    var newAuthorId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        FirstName = author.FirstName,
                        LastName = author.LastName,
                        Bio = author.Bio
                    });
                    return newAuthorId;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка CreateAuthorAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteAuthorAsync(int? id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    var query = "DELETE From Authors WHERE AuthorId = @AuthorId";
                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { AuthorId = id });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка DeleteAuthorAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            try
            {
                var query = "Select * from Authors";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<Author>(query);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении таблицы Authors {ex.Message}");
                throw new ApplicationException($"Ошибка при получении таблицы Authors {ex}");
            }
        }

        public async Task<IEnumerable<Author>> GetAuthorsByFNameOrLNameParametrAsync(string? FirstName, string? LastName)
        {
            try
            {
                if (!string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName))
                {
                    //получаем по FirstName
                    var query = "Select * from Authors where FirstName = @FirstName";

                    using (var connection = _context.CreateConnection())
                    {

                        return await connection.QueryAsync<Author>(query, new { FirstName = FirstName });
                    }
                }
                else
                {
                    var query = "Select * from Authors where LastName = @LastName";

                    using (var connection = _context.CreateConnection())
                    {

                        return await connection.QueryAsync<Author>(query, new { LastName = LastName });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Review по имени: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Review по имени id.");
            }
        }

        public async Task<Author> GetAuthorByIdAsync(int id) 
        {
            try 
            {
                if (id != null && id > 0)
                {
                    var query = "Select * from Authors where AuthorId = @AuthorId";

                    using (var connection = _context.CreateConnection())
                    {
                        //return await connection.QuerySingleOrDefaultAsync<Review>(query, new { ReviewId = id });
                        return await connection.QuerySingleOrDefaultAsync<Author>(query, new { AuthorId = id });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Author по id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Author.");
            }
        }

        public async Task<bool> UpdateAuthorAsync(Author author)
        {
            try
            {
                var query = "Update Authors " +
                    "Set FirstName = Coalesce(@FirstName,@FirstName), " +
                    "LastName = Coalesce(@LastName,LastName), " +
                    "Bio = Coalesce(@Bio,Bio) " +
                    "WHERE AuthorId = @AuthorId ";

                using (var connection = _context.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(query, author);
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка UpdateAuthorAsync {ex.Message}");
                return false;
            }
        }

        public async Task<Author> GetAuthorByFullNameAsync(string? FirstName, string? LastName)
        {
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName))
            {
                throw new ArgumentException("Параметры не должены быть пустыми");
            }
            try
            {
                //получаем по FirstName
                var query = "Select * from Authors where FirstName = @FirstName AND LastName = @LastName";

                using (var connection = _context.CreateConnection())
                {

                    return await connection.QueryFirstOrDefaultAsync<Author>(query, new { FirstName = FirstName, LastName = LastName });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Review по имени: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Review по имени id.");
            }
        }
    }
}
