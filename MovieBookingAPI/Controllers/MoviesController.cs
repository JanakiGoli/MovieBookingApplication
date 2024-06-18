using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using System.Net.Sockets;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieRepository _movieRepository;
        protected ILogger<MoviesController> _logger;


        public MoviesController(IMovieRepository movieRepository, ILogger<MoviesController> logger)
        {
            _movieRepository = movieRepository;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a list of all movies from the system.
        /// </summary>
        /// <returns>
        /// - 204 No Content if no movies are found.
        /// - 200 OK with a list of movies if retrieval is successful.
        /// </returns>


        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("all")]
       
        public async Task<ActionResult<List<Movie>>> GetAllMovies()
        {
            try
            {
                _logger.LogInformation($"Get all Movie names");

                var movies = await _movieRepository.GetMovies();
                if (movies.Count == 0)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(movies);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while fetching all movies: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Retrieves movies by their name from the system.
        /// </summary>
        /// <param name="moviename">The name of the movie to search for.</param>
        /// <returns>
        /// Returns a list of movies if found, or No Content if none are found.
        /// </returns>

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("search/{moviename}")]
       
        public async Task<ActionResult<List<Movie>>> GetByMovieName(string moviename)
        {
            try
            {
                _logger.LogInformation($"Getting by moviename : {moviename}");
                var movies = await _movieRepository.SearchMovie(moviename);
                if (movies.Count == 0)
                {
                    return NoContent();
                }
                else
                {
                    return Ok(movies);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while searching for movies by name: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Deletes a movie from a specific theatre.
        /// </summary>
        /// <param name="moviename">Name of the movie to delete.</param>
        /// <param name="theatrename">Name of the theatre from which to delete the movie.</param>
        /// <returns>
        /// Returns success message if deletion is successful, or error message if the movie doesn't exist.
        /// </returns>


        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("{moviename}/delete/{theatrename}")]
        

        public async Task<ActionResult<string>> DeleteMovie(string moviename, string theatrename)
        {
            try
            {
                _logger.LogInformation($"Delete movie : {moviename} at theatre : {theatrename}");

                var response = await _movieRepository.DeleteMovie(moviename, theatrename);
                if (response)
                {
                    return Ok($"{moviename} movie deleted successfully.");
                }
                else
                {
                    return BadRequest($"{moviename} movie doesn't exist.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while deleting the movie: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
