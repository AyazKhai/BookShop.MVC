using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IBookGenresRepos
    {
        Task<IEnumerable<BookGenre>> GetAllBookgenresAsync();
        Task<IEnumerable<Book>> GetAllBooksByGenreIdAsync(int genreid);
        Task<IEnumerable<Genre>> GetAllGenreByBookIdAsync(int bookid);
        Task<IEnumerable<Book>> GetAllBooksWithAllGenresAsync();//получает все книги и жанры

        Task<int> CreateBookGenreAsync(BookGenre bookGenre);

        Task<bool> DeleteBookGenresByBookIdAsync(int bookid);//удаляет все данные жанров по ид книги.Если книга больше не продается
       
        Task<bool> DeleteBookGenresByGenreIdAndBookIdAsync(int genreid, int bookid);//удаляет конкретный жанр
    }
}
