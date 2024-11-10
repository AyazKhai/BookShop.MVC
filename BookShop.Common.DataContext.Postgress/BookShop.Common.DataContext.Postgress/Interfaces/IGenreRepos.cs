using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IGenreRepos
    {
        Task<IEnumerable<Genre>> GetAllGenreAsync();
        Task<Genre> GetGenreByIdAsync(int? genreid);
        Task<int> CreateGenreAsync(Genre genre);
        Task<bool> UpdateGenreAsync(Genre genre);
        Task<bool> DeleteGenreAsync(int genreid);
    }
}
