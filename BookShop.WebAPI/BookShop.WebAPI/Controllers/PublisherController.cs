using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.WebAPI.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/publisher")]
    [ApiController]
    public class PublisherController : Controller
    {
        private readonly IPublisherRepos _publisherRepos;
        private readonly ILogger<PublisherController> _logger;

        public PublisherController(IPublisherRepos publisherRepos, ILogger<PublisherController> logger)
        {
            _publisherRepos = publisherRepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "PublisherControllerInitialization"), "PublisherController has been initialized.");
        }

        //GET: api/publisher?id=[]
        //GET: api/publisher
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Publisher))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPublisher(int? id)
        {
            try
            {
                if (id.HasValue && id > 0)
                {
                    var publisher = await _publisherRepos.GetPublisherByIdAsync(id);
                    if (publisher == null) 
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Publisher с id {id} wasnt found");
                        return NotFound();//404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Send Publisher with id {id} {publisher.Publishername}");
                    return Ok(publisher);//200
                }
                else if (!id.HasValue)
                {
                    var publishers = await _publisherRepos.GetAllPublisherAsync();
                    _logger.LogInformation(EventIds.Fetched, "Send all Publishers");
                    return Ok(publishers);
                }
                else 
                {
                    _logger.LogWarning(EventIds.Exception,$"Exception {ModelState.ValidationState}");
                    return BadRequest();
                }

                
            }
            catch (Exception ex)
            {
               _logger.LogError(EventIds.Error,ex, $"Internal server error.Error geting Publisher {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        ////POST: api/publisher
        ////BODY: Publisher(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(PublisherDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreatePublisher([FromBody] PublisherDto Publisher)
        {
            if (Publisher == null)
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Publisher. Publisher data is null") ;
                return BadRequest("Failed to create Publisher. Publisher data is null");
            }

            try
            {
                // Преобразуем DTO в основную модель Publisher
                var newPublisher = new Publisher
                {
                    Publishername = Publisher.Publishername,
                    Contactinfo = Publisher.Contactinfo,
                    Books = Publisher.Books
                    // PublisherId не устанавливаем, оно будет автоматически сгенерировано
                };

                var createdPublisherId = await _publisherRepos.CreatePublisherAsync(newPublisher);
                if (createdPublisherId > 0) // Проверяем, был ли создан издатель
                {
                    newPublisher.Publisherid = createdPublisherId;
                    _logger.LogInformation(EventIds.Created, $"Publisher has been created{newPublisher.Publishername}");
                    return StatusCode(201, new { Data = newPublisher});
                    //return CreatedAtAction(nameof(GetPublisher), new { id = createdPublisherId }, newPublisher);
                }
                else
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create Publisher");
                    return BadRequest("Failed to create Publisher.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating Publisher {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }

        //DELETE: api/publisher?id=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        public async Task<IActionResult> DeletePublisher(int id) 
        {

            if (id <= 0)
            {
                _logger.LogWarning(EventIds.Exception, "Invalid publisher ID.");
                return BadRequest("Invalid publisher ID.");
            }

            try
            {
                var deleted = await _publisherRepos.DeletePublisherAsync(id);
                if (!deleted)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Publisher with ID {id} not found.");
                    return NotFound($"Publisher with ID {id} not found.");
                }
                _logger.LogInformation(EventIds.Deleted, $"Publisher with ID {id} was deleted");
                return NoContent(); // Возвращаем статус 204 No Content при успешном удалении
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Publisher");
                return StatusCode(500, $"Internal server error.Error deleting Publisher ${ex.Message}");
            }
        }


        //PUT: api/publisher?id=[]
        //BODY: Publisher(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdatePublisher([Required]int id,[FromBody] Publisher updatedPublisher) 
        {
            if (updatedPublisher == null)
            {
                _logger.LogWarning(EventIds.Exception, "Publisher ID mismatch");
                return BadRequest("Publisher is null.");
            }

            if (id != updatedPublisher.Publisherid) // Проверяем соответствие ID
            {
                _logger.LogWarning(EventIds.Exception, "Publisher ID mismatch");
                return BadRequest("Publisher ID mismatch.");
            }
            try
            {
                var publisherExists = await _publisherRepos.GetPublisherByIdAsync(id);
                if (publisherExists == null)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Publisher with ID {id} not found.");
                    return NotFound($"Publisher with ID {id} not found.");
                }

                var updated = await _publisherRepos.UpdatePublisherAsync(updatedPublisher);
                if (!updated)
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update publisher.");
                    return BadRequest("Failed to update publisher.");
                }
                _logger.LogInformation(EventIds.Updated, $"Publisher with ID {id} was updated");
                return NoContent(); // Возвращаем статус 204 No Content при успешном обновлении
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error updating Publisher");
                return StatusCode(500, $"Internal server error.Error updating Publisher ${ex.Message}");
            }
        }
    }
}
