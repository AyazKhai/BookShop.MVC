using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.Models.Models;
using BookShop.WebAPI.Logging;
using Microsoft.AspNetCore.Mvc;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.Common.DataContext.Postgress.Repositories;
using System.ComponentModel.DataAnnotations;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/genre")]
    [ApiController]
    public class GenreController : Controller
    {
        private readonly IGenreRepos _genreRepos;
        private readonly ILogger<GenreController> _logger;

        public GenreController(IGenreRepos genreRepos, ILogger<GenreController> logger)
        {
            _genreRepos = genreRepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "GeenreControllerInitialization"),"GenreController has been initialized.");
        }


        //GET: api/genre?id=[]
        //GET: api/genre
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Genre))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetGenreAsync(int? genreid) 
        {
            try 
            {
                if (genreid.HasValue && genreid > 0)
                {
                    var genre = await _genreRepos.GetGenreByIdAsync(genreid);
                    if (genre == null)
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Genre с id {genreid} wasnt found");
                        return NotFound();//404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Send Genre with id {genreid} {genre.GenresName}");
                    return Ok(genre);//200
                }
                else if (!genreid.HasValue)
                {
                    var genre = await _genreRepos.GetAllGenreAsync();
                    _logger.LogInformation(EventIds.Fetched, "Send all Genres");
                    return Ok(genre);
                }
                else 
                {
                    _logger.LogWarning(EventIds.Exception,$"Exceeption {ModelState.ValidationState}");
                    return BadRequest();
                }
            }
            catch (Exception ex) 
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting Genre {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        ////POST: api/genre
        ////BODY: Genre(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(GenreDto))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateGenreAsync([FromBody] GenreDto Genre) 
        {
            if (Genre == null) 
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Genre. Genre data is null");
                return BadRequest("Failed to create Genre. Genre data is null");
            }

            try
            {
                var newGenre = new Genre
                {
                    GenresName = Genre.GenresName,
                    Description = Genre.Description
                };

                var createdGenreId = await _genreRepos.CreateGenreAsync(newGenre);
                if (createdGenreId > 0)
                {
                    newGenre.GenreId = createdGenreId;
                    _logger.LogInformation(EventIds.Created, $"Genre has been created {newGenre.GenresName}");
                    return StatusCode(201, new { Data = newGenre });
                    //return CreatedAtAction(nameof(GetGenreAsync), new { id = createdGenreId }, newGenre);
                }
                else 
                {
                    _logger.LogWarning(EventIds.Exception, "Alredy exists");
                    return BadRequest("Alredy exists");
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating Genre {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }

        //DELETE: api/genre?id=[]
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        public async Task<IActionResult> DeleteGenreAsync(int genreid)
        {
            if (genreid <= 0)
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Genre ID.");
                return BadRequest("Invalid Genre ID");
            }

            try
            {
                var deleted = await _genreRepos.DeleteGenreAsync(genreid);
                if (!deleted)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Genre with ID {genreid} not found.");
                    return NotFound($"Genre with ID {genreid} not found");
                }
                _logger.LogInformation(EventIds.Deleted, $"Genre with ID {genreid} was deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Genre");
                return StatusCode(500, $"Internal server error.Error deleting Genre ${ex.Message}");
            }
        }

        //PUT: api/genre?genreid=[]
        //BODY: Genre(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateCustomerAsync([Required]int genreid, [FromBody] Genre updatedGenre)
        {
            if (updatedGenre == null)
            {
                _logger.LogWarning(EventIds.Exception, "Genre ID mismatch");
                return BadRequest("Genre is null");
            }

            if (genreid != updatedGenre.GenreId)
            {
                _logger.LogWarning(EventIds.Exception, "Genre ID mismatch");
                return BadRequest("Genre ID mismatch");
            }

            try
            {
                var genreExsists = await _genreRepos.GetGenreByIdAsync(genreid);
                if (genreExsists == null)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Genre with ID {genreid} not found.");
                    return NotFound($"Genrewith ID {genreid} not found");
                }

                var updated = await _genreRepos.UpdateGenreAsync(updatedGenre);
                if (!updated)
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update Genre.");
                    return BadRequest("Failed to update Genre");
                }

                _logger.LogInformation(EventIds.Updated, $"Genre with ID {genreid} was updated");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error updating Genre");
                return StatusCode(500, $"Internal server error.Error updating Genre ${ex.Message}");
            }
        }
    }
}
