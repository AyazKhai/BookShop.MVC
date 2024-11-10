using BookShop.Common.DataContext.Postgress.Interfaces;
using BookShop.Common.DataContext.Postgress.Repositories;
using BookShop.Common.Models.Models;
using BookShop.WebAPI.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_;
using BookShop.Common.Models.Postgress.DTO_Data_Transfer_Object_.ForUpadationData;
using System.Net.Mime;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace BookShop.WebAPI.Controllers
{
    [Route("api/review")]
    [ApiController]
    public class ReviewController : Controller
    {
        // private readonly ICustomerRepos _customerRepos;
        private readonly IReviewRepos _reviewRepos;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewRepos reviewRepos, ILogger<ReviewController> logger)
        {
            _reviewRepos = reviewRepos;
            _logger = logger;
            _logger.LogDebug(new EventId(1, "ReviewControllerInitialization"), "ReviewController has been initialized.");
        }


        //GET: api/review?id=[]&customerid=[]&bookid=[]
        //GET: api/review?id=[]&customerid=[]
        //GET: api/review?id=[]&bookid=[]
        //GET: api/review?customerid=[]
        //GET: api/review?bookid=[]
        //GET: api/review?id=[]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewsAsync([FromQuery]int? id, [FromQuery]int? customerId, [FromQuery]int? bookId)
        {
            try
            {
                // Проверка, что передан только один параметр
                int paramCount = (id.HasValue ? 1 : 0) + (customerId.HasValue ? 1 : 0) + (bookId.HasValue ? 1 : 0);

                if (paramCount > 1)
                {
                    _logger.LogWarning(EventIds.Exception, "Only one query parameter (id, customerId, or bookId) can be used at a time.");
                    return BadRequest("Only one query parameter (id, customerId, or bookId) can be used at a time.");
                }

                if (id.HasValue && id > 0)
                {
                    var review = await _reviewRepos.GetReviewByIdAsync(id.Value);
                    if (review == null || !review.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"Review with id {id} wasn't found");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent review with id {id}");
                    return Ok(review); // 200
                }

                if (customerId.HasValue && customerId > 0)
                {
                    var reviews = await _reviewRepos.GetReviewByCustomerIdAsync(customerId.Value);
                    if (reviews == null || !reviews.Any())
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No reviews found for customer with id {customerId}");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent reviews for customer id {customerId}");
                    return Ok(reviews); // 200
                }

                if (bookId.HasValue && bookId > 0)
                {
                    var reviews = await _reviewRepos.GetReviewByBookIdAsync(bookId.Value);
                    if (reviews == null || !reviews.Any() )
                    {
                        _logger.LogWarning(EventIds.NotFound, $"No reviews found for book with id {bookId}");
                        return NotFound(); // 404
                    }
                    _logger.LogInformation(EventIds.Fetched, $"Sent reviews for book id {bookId}");
                    return Ok(reviews); // 200
                }

                // Если не указаны параметры, возвращаем все отзывы
                var allReviews = await _reviewRepos.GetAllReviewAsync();
                _logger.LogInformation(EventIds.Fetched, "Sent all reviews");
                return Ok(allReviews); // 200
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error geting Review {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }



        ////POST: api/review?customerid=[]&bookid=[]
        ////BODY: Review(JSON)
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(ReviewDto))]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReviewAsync([FromBody] ReviewDto newReviewDto, [Required][FromQuery] int customerId, [Required][FromQuery] int bookid) 
        {
            if (newReviewDto == null)
            {
                _logger.LogWarning(EventIds.Exception, "Failed to create Review. Review data is null");
                return BadRequest("Failed to create Review. Review data is null");
            }

            if (customerId <= 0) 
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Customer ID.");
                return BadRequest("Invalid Customer ID");
            }
            if (bookid <= 0) 
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Book ID.");
                return BadRequest("Invalid Book ID");
            }
            try 
            {
                
                var newReview = new Review 
                {
                    CustomerId = customerId,
                    BookId = bookid,
                    Rating = newReviewDto.Rating,
                    CommentText = newReviewDto.CommentText,
                };
                var createdReviewId = await _reviewRepos.CreateReviewAsync(newReview);
                if (createdReviewId > 0) 
                {
                    newReview.ReviewId = createdReviewId;
                    _logger.LogInformation(EventIds.Created, $"Review has been created ");
                    return StatusCode(201, new { Data = newReview });
                }
                else 
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to create Review");
                    return BadRequest("Failed to create Review");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, $"Internal server error.Error creating Review {ex.Message}");
                return StatusCode(500, ex.Message); // 500
            }
        }

        //DELETE: api/review?id=reviewid
        [HttpDelete]
        [ProducesResponseType(204)] // Успешное удаление
        [ProducesResponseType(404)] // Ресурс не найден
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteReviewAsync([FromQuery][Required] int reviewId) 
        {
            if (reviewId <= 0)
            {
                _logger.LogWarning(EventIds.Exception, "Invalid Review ID.");
                return BadRequest("Invalid Review ID");
            }
            try
            {
                var deleted = await _reviewRepos.DeleteReviewAsync(reviewId);
                if (!deleted)
                {
                    _logger.LogWarning(EventIds.NotFound, $"Review with ID {reviewId} not found.");
                    return NotFound($"Review with ID {reviewId} not found");
                }
                _logger.LogInformation(EventIds.Deleted, $"Review with ID {reviewId} was deleted");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error deleting Review");
                return StatusCode(500, $"Internal server error.Error deleting Review ${ex.Message}");
            }
        }


        //PUT: api/review?reviewid=[]
        //BODY: Review(JSON)
        [HttpPut]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateReviewAsync([FromQuery] int reviewid, [FromBody] ReviewUpdateDto updatedReview)
        {
            if (updatedReview == null)
            {
                _logger.LogWarning(EventIds.Exception, "Review is null");
                return BadRequest("Review is null");
            }

            if (reviewid != updatedReview.ReviewId)
            {
                _logger.LogWarning(EventIds.Exception, "Review ID mismatch");
                return BadRequest("Review ID mismatch");
            }
            try
            {
                var reviewExists = await _reviewRepos.GetReviewByIdAsync(reviewid);
                if (reviewExists == null) 
                {
                    _logger.LogWarning(EventIds.NotFound, $"Review with ID {reviewid} not found.");
                    return NotFound($"Review with ID {reviewid} not found");
                }

                var updated = await _reviewRepos.UpdateReviewAsync(updatedReview);
                if (!updated)
                {
                    _logger.LogWarning(EventIds.Exception, "Failed to update Review.");
                    return BadRequest("Failed to update Review");
                }

                _logger.LogInformation(EventIds.Updated, $"Review with ID {reviewid} was updated");
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(EventIds.Error, ex, "Error updating Review");
                return StatusCode(500, $"Internal server error.Error updating Review ${ex.Message}");
            }
        }
    }
}




