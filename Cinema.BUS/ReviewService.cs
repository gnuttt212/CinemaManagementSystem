using Cinema.DAL;
using Cinema.DAL.Models.Mongo;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cinema.BUS
{
    public interface IReviewService
    {
        Task<List<MovieReview>> GetReviewsByMovieAsync(int maPhim);
        Task CreateReviewAsync(MovieReview review);
    }

    public class ReviewService : IReviewService
    {
        private readonly MongoDbContext _context;

        public ReviewService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<MovieReview>> GetReviewsByMovieAsync(int maPhim)
        {
            return await _context.MovieReviews.Find(r => r.MaPhim == maPhim).SortByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task CreateReviewAsync(MovieReview review)
        {
            await _context.MovieReviews.InsertOneAsync(review);
        }
    }
}
