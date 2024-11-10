using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IAuthorRepos
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task<IEnumerable<Author>> GetAuthorsByFNameOrLNameParametrAsync(string? FirstName, string? LastName);
        Task<Author> GetAuthorByIdAsync(int id);
        Task<Author> GetAuthorByFullNameAsync(string? FirstName, string? LastName);

        Task<int> CreateAuthorAsync(Author author);

        Task<bool> UpdateAuthorAsync(Author author);
        Task<bool> DeleteAuthorAsync(int? id);
    }
}
