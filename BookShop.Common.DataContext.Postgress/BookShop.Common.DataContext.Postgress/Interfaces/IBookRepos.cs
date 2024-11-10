using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IBookRepos
    {
        Task<int> CreateBookAsync(Book book);
        Task <IEnumerable<Book>> GetAllBoksAsync();
        Task<IEnumerable<Book>> GetAllBooksByTitleAsync(string title);
        Task<IEnumerable<Book>> GetBooksArrayAsync(int limit,int offset, string sort, string order, int? genreid);
        Task<Book?> GetAllBooksByIdAsync(int? id);

        Task<bool> UpdateBookAsync(BookUpdateDto book);

        Task<bool> DeleteBookAsync(int id);
        Task<bool> DeleteBookByTitleAsync(string title);
        Task<bool> DeleteBookByIsbnAsync(string isbn);

    }
}
