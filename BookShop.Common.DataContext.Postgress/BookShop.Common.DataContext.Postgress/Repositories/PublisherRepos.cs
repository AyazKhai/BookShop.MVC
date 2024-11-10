using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class PublisherRepos : IPublisherRepos
    {
        DapperDbContext _context;
        public PublisherRepos(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreatePublisherAsync(Publisher publisher)
        {
            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher), "Publisher cannot be null");
            }

            try
            {
                var query = @"
                    INSERT INTO publishers (publishername, contactinfo)
                    VALUES (@PublisherName, @ContactInfo)
                    RETURNING publisherid;";

                using (var connection = _context.CreateConnection())
                {
                    var newPublisherId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        PublisherName = publisher.Publishername,
                        ContactInfo = publisher.Contactinfo
                    });
                    return newPublisherId;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Ошибка CreatePublisherAsync: {ex.Message}");
                throw ex;
            }
            
        }

        public async Task<bool> DeletePublisherAsync(int publisherId)
        {
            try 
            {
                if (publisherId != null && publisherId > 0) 
                {
                    var query = "DELETE FROM Publishers WHERE publisherid = @publishersid";

                    using (var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { publishersid = publisherId });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine("Ошибка DeletePublisherAsync.", ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<Publisher>> GetAllPublisherAsync()
        {
            try 
            {
                var query = "Select * from Publishers";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<Publisher>(query);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Publishers: {ex.Message}");
                // Вариант: выбросить исключение дальше или вернуть пустую коллекцию
                throw new ApplicationException("Ошибка при получении списка Publishers.", ex);
                // или
                // return Enumerable.Empty<Publisher>();
            }

        }

        public async Task<Publisher?> GetPublisherByIdAsync(int? publisherId)
        {

            try 
            {
                if (publisherId != null && publisherId > 0) 
                {
                    var query = "Select * from Publishers where publisherid = @publishersid";

                    using (var connection = _context.CreateConnection())
                    {
                        return await connection.QuerySingleOrDefaultAsync<Publisher>(query, new { publishersid = publisherId });
                    }
                }
                return null;
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Publishers по id: {ex.Message}");
               
                throw new ApplicationException("Ошибка при получении пользователя Publishers.", ex);
            }
            
        }

        public async Task<bool> UpdatePublisherAsync(Publisher publisher)
        {
            try 
            {
                var query = @"
            UPDATE Publishers 
            SET PublisherName = COALESCE(@Publishername, PublisherName),
                ContactInfo = COALESCE(@Contactinfo, ContactInfo)
            WHERE Publisherid = @Publisherid";

                using (var connection = _context.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(query, publisher);
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка UpdatePublisherAsync {ex.Message}");
                return false;   
            }
        }
    }
}


/*
 public class UserRepository : IUserRepository
{
    private readonly DapperContext _context;

    public UserRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var query = "SELECT * FROM Users";

        using (var connection = _context.CreateConnection())
        {
            return await connection.QueryAsync<User>(query);
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var query = "SELECT * FROM Users WHERE UserId = @UserId";

        using (var connection = _context.CreateConnection())
        {
            return await connection.QuerySingleOrDefaultAsync<User>(query, new { UserId = userId });
        }
    }

    public async Task<int> CreateUserAsync(User user)
    {
        var query = @"
            INSERT INTO Users (Username, Email, PasswordHash, CreatedAt, UpdatedAt)
            VALUES (@Username, @Email, @PasswordHash, @CreatedAt, @UpdatedAt)
            RETURNING UserId";

        using (var connection = _context.CreateConnection())
        {
            return await connection.QuerySingleAsync<int>(query, user);
        }
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        var query = @"
            UPDATE Users 
            SET Username = @Username, 
                Email = @Email, 
                PasswordHash = @PasswordHash, 
                UpdatedAt = @UpdatedAt 
            WHERE UserId = @UserId";

        using (var connection = _context.CreateConnection())
        {
            var affectedRows = await connection.ExecuteAsync(query, user);
            return affectedRows > 0;
        }
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var query = "DELETE FROM Users WHERE UserId = @UserId";

        using (var connection = _context.CreateConnection())
        {
            var affectedRows = await connection.ExecuteAsync(query, new { UserId = userId });
            return affectedRows > 0;
        }
    }
}
 */