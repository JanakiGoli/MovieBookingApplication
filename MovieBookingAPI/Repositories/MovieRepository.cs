using MongoDB.Bson;
using MongoDB.Driver;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;

namespace MovieBookingAPI.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly IMongoCollection<Movie> _movies;
        private readonly IMongoCollection<Ticket> _tickets;
        protected ILogger<MovieRepository> _logger;


        public MovieRepository(IMongoDbConfig config,IMongoClient mongoClient, ILogger<MovieRepository> logger)
        {
            var database = mongoClient.GetDatabase(config.DatabaseName);
            _movies=database.GetCollection<Movie>(config.MovieCollectionName);
            _tickets=database.GetCollection<Ticket>(config.TicketCollectionName);
            _logger = logger;

        }
        
        public async Task<List<Movie>> GetMovies()
        {
            _logger.LogInformation($"Get all Movie ");

            var movies = await _movies.FindAsync(m=>true);
            return movies.ToList();
        }

        public async Task<List<Movie>> SearchMovie(string movieName)
        {
            _logger.LogInformation($"Search by moviename : {movieName}");

            var movies = await _movies.FindAsync(Builders<Movie>.Filter.
                Regex("moviename",new BsonRegularExpression(movieName,"i")));
            return movies.ToList();
        }

        public async Task<bool> DeleteMovie(string moviename,string theatreName)
        {
            _logger.LogInformation($"Delete movie : {moviename} at theatre : {theatreName}");

            var result = await _movies.DeleteOneAsync(x=>x.MovieName.ToLower() == moviename.ToLower() && x.TheatreName.ToLower() == theatreName.ToLower());

            await _tickets.DeleteManyAsync(x => x.MovieName.ToLower() == moviename.ToLower() && x.TheatreName.ToLower() == theatreName.ToLower());
            if (result.DeletedCount ==0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
    }
}


