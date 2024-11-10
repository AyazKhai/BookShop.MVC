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
    public class GenreRepos : IGenreRepos
    {
        DapperDbContext _context;
        public GenreRepos(DapperDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateGenreAsync(Genre genre)
        {
            if (genre == null) 
            {
                throw new ArgumentNullException(nameof(genre),"Genre cannot be null");
            }
            try 
            {
                var query = @"INSERT INTO Genres(GenresName,Description)
                                Values(@GenresName,@Description)
                                RETURNING GenreId";

                using(var connection = _context.CreateConnection()) 
                {
                    var newGenreId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        GenresName = genre.GenresName,
                        Description = genre.Description 
                    });
                    return newGenreId;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка CreateGenreAsync: {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteGenreAsync(int genreId)
        {
            try
            {
                if (genreId != null && genreId > 0) 
                {
                    var query = "DELETE FROM Genres WHERE Genreid = @Genreid";
                    using(var connection = _context.CreateConnection()) 
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { GenreId = genreId });
                        return affectedRows > 0;
                    }
                }
                return false;
            } 
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка DeleteGenreAsync {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Genre>> GetAllGenreAsync()
        {
            try 
            {
                var query = "Select * from Genres";

                using(var connection = _context.CreateConnection()) 
                {
                    return await connection.QueryAsync<Genre>(query);
                }
            }
            catch(Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка при получении Genres {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Genres {ex}");
            }
        }

        public async Task<Genre> GetGenreByIdAsync(int? genreId)
        {
            try
            {
                if (genreId != null && genreId > 0) 
                {
                    var query = "Select * from Genres where GenreId = @GenreId";

                    using (var connection = _context.CreateConnection()) 
                    {
                        return await connection.QuerySingleOrDefaultAsync<Genre>(query, new { GenreId = genreId });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Genre по id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Genre.");
            }
        }

        public async Task<bool> UpdateGenreAsync(Genre genre)
        {
            try 
            {
                var query = "Update Genres " +
                    "Set GenresName = COALESCE(@GenresName,GenresName), " +
                    "Description = COALESCE(@Description,Description) " +
                    "where GenreId = @GenreId";

                using(var connection = _context.CreateConnection()) 
                {
                    var affectedRows = await connection.ExecuteAsync(query, genre);
                    return affectedRows > 0;
                }
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка UpdateGenreAsync {ex.Message}");
                return false;
            }
        }
    }
}
