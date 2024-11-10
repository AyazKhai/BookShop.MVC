using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface IPublisherRepos
    {
        Task<IEnumerable<Publisher>> GetAllPublisherAsync();
        Task<Publisher?> GetPublisherByIdAsync(int? publisherId);
        Task<int> CreatePublisherAsync(Publisher publisher);
        Task<bool> UpdatePublisherAsync(Publisher publisher);
        Task<bool> DeletePublisherAsync(int publisherId);
    }
}
