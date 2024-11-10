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
    public class CustomerRepos : ICustomerRepos
    {
        DapperDbContext _context;
        public CustomerRepos(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentNullException(nameof(customer), "Customer cannot be null");
            }

            try
            {
                var query = @"
                    INSERT INTO Customers(FirstName,LastName,Email,PhoneNumber,Address)
                    Values (@FirstName, @LastName, @Email, @PhoneNumber, @Address)
                    RETURNING CustomerId;";

                using (var connection = _context.CreateConnection())
                {
                    var newCustomerrId = await connection.ExecuteScalarAsync<int>(query, new
                    {
                        FirstName = customer.FirstName, 
                        LastName = customer.LastName,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber,
                        Address = customer.Address

                    });
                    return newCustomerrId;
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ошибка CreateCustomerAsync: {ex.Message}");
                throw ex;
            }
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            try 
            {
                if(customerId != null && customerId > 0)
                {
                    var query = "DELETE FROM Customers WHERE Customerid = @CustomerId";

                    using(var connection = _context.CreateConnection())
                    {
                        var affectedRows = await connection.ExecuteAsync(query, new { CustomerId = customerId });
                        return affectedRows > 0;
                    }
                }
                return false;
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка DeleteCustomerAsync {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Customer>> GetAllCustomerAsync()
        {
            try 
            {
                var query = "SELECT * FROM Customers" +
                    " ORDER BY customerid ASC ";

                using (var connection = _context.CreateConnection()) 
                {
                    return await connection.QueryAsync<Customer>(query);
                }
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка при получении Customers {ex.Message}");
                throw new ApplicationException($"Ошибка при получении списка Customers {ex}");
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int? customerid)
        {
            try 
            {
                if (customerid != null && customerid > 0) 
                {
                    var query = "SELECT * FROM Customers WHERE CustomerId = @CustomerId";

                    using (var connection = _context.CreateConnection()) 
                    {
                        return await connection.QuerySingleOrDefaultAsync<Customer>(query, new {CustomerId = customerid});
                    }
                }
                return null;
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка при получении данных из таблицы Customers по id: {ex}");
                throw new ApplicationException("Ошибка при получении пользователя Customers.");
            }
        }

        public async Task<bool> UpdateCustomerAsync(Customer customer)
        {
            try 
            {
                var query = "Update Customers Set FirstName = COALESCE(@FirstName, FirstName)," +
                    "LastName= COALESCE(@LastName, LastName)," +
                    "Email= COALESCE(@Email,Email)," +
                    "PhoneNumber= COALESCE(@PhoneNumber,PhoneNumber), " +
                    "Address= COALESCE(@Address,Address) " +
                    "WHERE Customerid = @CustomerId";

                using (var connection = _context.CreateConnection()) 
                {
                    var affectedRows = await connection.ExecuteAsync(query, customer);
                    return affectedRows > 0;
                } 
            }
            catch (Exception ex) 
            {
                Console.Error.WriteLine($"Ошибка UpdateCustomerAsync {ex.Message}");
                return false;
            }
        }
    }
}
