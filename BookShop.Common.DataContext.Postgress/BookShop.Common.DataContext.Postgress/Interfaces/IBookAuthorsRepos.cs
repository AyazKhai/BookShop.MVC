using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookShop.Common.Models.Models;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IBookAuthorsRepos
    {
        Task<IEnumerable<BookAuthor>> GetAllBookAuthorsAsync();
       
        Task<IEnumerable<Author>> GetAllAuthorsByBookIdAsync(int bookid);
       
        Task<IEnumerable<Book>> GetAllBooksByAuthorIdAsync(int authorid);
        Task<IEnumerable<Book>> GetAllBooksWithAuthorsAsync();

        Task<int> CreateBookAuthorAsync(BookAuthor author);
        Task<bool> DeleteBookAuthorsByBookId(int bookid);
        Task<bool> DeleteBookAuthorByBookIdAndAuthorId(int bookid, int authorid);
    }
}
