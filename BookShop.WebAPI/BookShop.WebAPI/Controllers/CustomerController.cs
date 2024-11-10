using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.DataContext.Postgress.Repositories;
using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.WebAPI.Logging;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepos _customerRepos;
        private readonly ILogger<CustomerController> _logger;
        public CustomerController(ICustomerRepos customerRepos, ILogger<CustomerController> logger)
        {
            _customerRepos = customerRepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "CustomerControllerInitialization"), "CustomerController has been initialized.");
        }

        //GET: api/customer/?id = [id]
        //GET: api/customer
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Publisher))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCustomerAsync(int? id)
        {
            try
            {
                if (id.HasValue && id > 0)
                {
                    var customer = await _customerRepos.GetCustomerByIdAsync(id);
                    if (customer == null)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Customer с id {id} wasnt found");
                        return NotFound();//404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Send  Customer with id {id} {customer.FirstName} {customer.LastName}");
                    return Ok(customer);//200
                }
                else if (!id.HasValue)
                {
                    var customers = await _customerRepos.GetAllCustomerAsync();
                    _logger.LogInformation(EventIds.Fetched, "Send all Customers");
                    return Ok(customers);
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, $"Exception {ModelState.ValidationState}");
                    return BadRequest();
                }


            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting  Customer {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        ////POST: api/customer
        ////BODY: Customer(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Customer))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCustomerAsync([FromBody] CustomerDto Customer)
        {
            if (Customer == null)
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Customer. Customer data is null");
                return BadRequest("Failed to create Customer. Customer data is null");
            }


            try
            {
                // Преобразуем DTO в основную модель Customer
                var newCustomer = new Customer
                {
                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    Email = Customer.Email,
                    PhoneNumber = Customer.PhoneNumber,
                    Address = Customer.Address
                };

                var createdCustomerId = await _customerRepos.CreateCustomerAsync(newCustomer);
                if (createdCustomerId > 0) // Проверяем, был ли создан 
                {
                    newCustomer.CustomerId = createdCustomerId;
                    _logger.LogInformation(EventIds.Created, $"Customer has been created {newCustomer.FirstName} {newCustomer.LastName}");
                    return StatusCode(201, new {Data = newCustomer });
                    //return CreatedAtAction(nameof(GetCustomerAsync), new { id = createdCustomerId }, newCustomer);
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create Customer");
                    return BadRequest("Failed to create Customer.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating Customer {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }


        //DELETE: api/customer?id=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        public async Task<IActionResult> DeleteCustomerAsync([Required] int id) 
        {
            if (id <= 0) 
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Customer ID.");
                return BadRequest("Invalid Customer ID");
            }

            try 
            {
                var deleted = await _customerRepos.DeleteCustomerAsync(id);
                if(!deleted) 
                {
                    _logger.LogWarning(EventIds.NotFound, $"Customer with ID {id} not found.");
                    return NotFound($"Customer with ID {id} not found");
                }
                _logger.LogInformation(EventIds.Deleted, $"Customer with ID {id} was deleted");
                return NoContent();
            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Customer");
                return StatusCode(500, $"Internal server error.Error deleting Customer ${ex.Message}");
            }
        }

        //PUT: api/customer?id=[]
        //BODY: Customer(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomerAsync([Required]int id, [FromBody] Customer updatedCustomer) 
        {
            if (updatedCustomer == null) 
            {
                _logger.LogWarning(EventIds.Exception, "Customer ID mismatch");
                return BadRequest("Customer is null");
            }

            if(id != updatedCustomer.CustomerId) 
            {
                _logger.LogWarning(EventIds.Exception, "Customer ID mismatch");
                return BadRequest("Customer ID mismatch");
            }

            try 
            {
                var customerExsists = await _customerRepos.GetCustomerByIdAsync(id);
                if (customerExsists == null)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Customer with ID {id} not found.");
                    return NotFound($"Customer with ID {id} not found");
                }

                var updated = await _customerRepos.UpdateCustomerAsync(updatedCustomer);
                if (!updated) 
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update Customer.");
                    return BadRequest("Failed to update Customer");
                }
                    
                _logger.LogInformation(EventIds.Updated, $"Customer with ID {id} was updated");
                return NoContent();
            }
            catch(Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, "Error updating Customer");
                return StatusCode(500, $"Internal server error.Error updating Customer ${ex.Message}");
            }
        }
    }
}
