using BookShop.Common.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookShop.Common.DataContext.Postgress.Interfaces
{
    public interface ICustomerRepos
    {
        Task<IEnumerable<Customer>> GetAllCustomerAsync();
        Task<Customer?> GetCustomerByIdAsync(int? customerid);
        Task<int> CreateCustomerAsync(Customer customer);
        Task<bool> UpdateCustomerAsync(Customer customer);
        Task<bool> DeleteCustomerAsync(int customerId);
    }
}
