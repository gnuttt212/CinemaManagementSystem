using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Identity.Data;
using Cinema.Web.Modules.Catalog.Entities;
using Cinema.Web.Modules.Identity.Services;
using Cinema.Web.Modules.Catalog.Services;
using Cinema.Web.Modules.Booking.Services;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Cinema.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{maPhim}")]
        public async Task<IActionResult> GetReviews(int maPhim)
        {
            var reviews = await _reviewService.GetReviewsByMovieAsync(maPhim);
            return Ok(reviews);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostReview([FromBody] MovieReview review)
        {
            var userAccount = HttpContext.Session.GetString("UserAccount");
            if (string.IsNullOrEmpty(userAccount))
            {
                return Unauthorized(new { message = "Vui lòng đăng nhập để đánh giá." });
            }

            review.UserAccount = userAccount;
            await _reviewService.CreateReviewAsync(review);
            
            return Ok(new { success = true });
        }
    }
}




