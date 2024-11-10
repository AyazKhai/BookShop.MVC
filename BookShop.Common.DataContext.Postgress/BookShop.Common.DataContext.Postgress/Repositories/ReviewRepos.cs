using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Repositories
{
    public class ReviewRepos : IReviewRepos
    {
        DapperDbContext _context;
        public ReviewRepos(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateReviewAsync(Review review)
        {
            if (review == null) 
            {
                throw new ArgumentNullException(nameof(Review), "Review cannot be null");
            }
            try 
            {
                var query = "INSERT INTO Reviews(CustomerId,BookId,Rating,CommentText) " +
                    "Values(@CustomerId,@BookId,@Rating,@CommentText) RETURNING ReviewId";

                using (var connection = _context.CreateConnection()) 
                {
                    var newReviewId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        CustomerId = review.CustomerId,
                        BookId = review.BookId,
                        Rating = review.Rating,
                        ComentText = review.CommentText
                    });
                    return newReviewId;
                }
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка CreateReviewAsync {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteReviewAsync(int? id)
        {
            try 
            {
                if (id != null && id > 0) 
                {
                    var query = "DELETE FROM Reviews WHERE ReviewId = @ReviewId ";
                    using (var connection = _context.CreateConnection()) 
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { ReviewId = id });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch(Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка DeletereviewAsync {ex.Message}");
                throw ex;
            }
        }
        public async Task<bool> UpdateReviewAsync(ReviewUpdateDto review)
        {
            try
            {
                var query = "Update Reviews " +
                    "Set Rating = Coalesce(@Rating,Rating), " +
                    "Commenttext = Coalesce(@Commenttext,Commenttext), " +
                    "reviewdate = CURRENT_DATE " +
                    "WHERE ReviewId = @ReviewId";

                using (var connection = _context.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(query, review);
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка UpdateReviewAsync {ex.Message}");
                return false;
            }
        }


        public async Task<IEnumerable<Review>> GetAllReviewAsync()
        {
            try
            {
                var query = "Select * from Reviews";

                using (var connection = _context.CreateConnection())
                {
                    return await connection.QueryAsync<Review>(query);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении списка Reviews {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Reviews {ex}");
            }
        }

        public async Task<IEnumerable<Review>> GetReviewByBookIdAsync(int? id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    var query = "Select * from Reviews where BookId = @BookId";

                    using (var connection = _context.CreateConnection())
                    {
                        //return await connection.QuerySingleOrDefaultAsync<Review>(query, new { ReviewId = id });
                        return await connection.QueryAsync<Review>(query, new { BookId = id });
                    }
                    //GetReviewByCriteriaAsync("CustomerId", id);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Review по Book id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Review по Book id.");
            }
        }

        public async Task<IEnumerable<Review>> GetReviewByCustomerIdAsync(int? id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    var query = "Select * from Reviews where CustomerId = @CustomerId";

                    using (var connection = _context.CreateConnection())
                    {
                        //return await connection.QuerySingleOrDefaultAsync<Review>(query, new { ReviewId = id });
                        return await connection.QueryAsync<Review>(query, new { CustomerId = id });
                    }
                    //GetReviewByCriteriaAsync("CustomerId", id);
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Review по Customer id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Review по Customer id.");
            }
        }

        public async Task<IEnumerable<Review>> GetReviewByIdAsync(int? id)
        {
            try
            {
                if (id != null && id > 0)
                {
                    var query = "Select * from Reviews where ReviewId = @ReviewId";

                    using (var connection = _context.CreateConnection())
                    {
                        //return await connection.QuerySingleOrDefaultAsync<Review>(query, new { ReviewId = id });
                        return await connection.QueryAsync<Review>(query, new { ReviewId = id });
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Review по id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Review.");
            }
        }

        
    }
}
