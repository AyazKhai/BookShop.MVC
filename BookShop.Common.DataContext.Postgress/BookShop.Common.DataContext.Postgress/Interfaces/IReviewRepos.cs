using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IReviewRepos
    {
        Task<IEnumerable<Review>> GetAllReviewAsync();
        Task<IEnumerable<Review>> GetReviewByIdAsync(int? id);
        Task<IEnumerable<Review>> GetReviewByBookIdAsync(int? id);
        Task<IEnumerable<Review>> GetReviewByCustomerIdAsync(int? id);

        Task<int> CreateReviewAsync(Review review);
        Task<bool> UpdateReviewAsync(ReviewUpdateDto review);
        Task<bool> DeleteReviewAsync(int? id);

    }
}
