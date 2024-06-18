using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieBookingAPI.Interfaces;
using MovieBookingAPI.Models;
using System.Data;

namespace MovieBookingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;
        protected ILogger<TicketsController> _logger;

        public TicketsController(ITicketRepository ticketRepository, ILogger<TicketsController> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        /// <summary>
        /// Books tickets for a movie at a specified theatre.
        /// </summary>
        /// <param name="ticket">The ticket object containing movie and theatre information.</param>
        /// <returns>
        /// Returns a success message if booking is successful, or an error message if booking fails.
        /// </returns>


        [Authorize(Roles = "Admin, Member")]
        [HttpPost]
        [Route("booktickets")]
       
        public async Task<ActionResult> BookTickets([FromBody] Ticket ticket)
        {
            try
            {
                _logger.LogInformation($"Booking tickets for movie : {ticket.MovieName} at theatre : {ticket.TheatreName}");

                var response = await _ticketRepository.BookTickets(ticket);
                if (response == -1)
                {
                    return BadRequest("Booking failed as there are no requested number of seats.");
                }
                else if (response == 0)
                {
                    return BadRequest("You are trying to book an already booked seat.");
                }
                return Ok("Booked Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while booking tickets: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Updates the ticket status for a movie at a specified theatre.
        /// </summary>
        /// <param name="moviename">Name of the movie.</param>
        /// <param name="theatrename">Name of the theatre.</param>
        /// <returns>
        /// Returns the updated ticket status.
        /// </returns>

        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("{moviename}/update/{theatrename}")]


        public ActionResult UpdateTicketStatus(string moviename, string theatrename)
        {
            try
            {
                _logger.LogInformation($"Update ticket status for movie : {moviename} at theatre : {theatrename}");

                var status = _ticketRepository.UpdateTicketStatus(moviename, theatrename);
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while updating ticket status for movie : {moviename} at theatre : {theatrename}. Error: {ex.Message}");
                return StatusCode(500, "Please enter valid moviename and theatrename");
            }
        }


        /// <summary>
        /// Retrieves booking information for a movie at a specified theatre.
        /// </summary>
        /// <param name="moviename">Name of the movie.</param>
        /// <param name="theatrename">Name of the theatre.</param>
        /// <returns>
        /// Returns the total available seats and booked tickets for the movie at the specified theatre.
        /// </returns>


        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("{moviename}/getBookingInfo/{theatrename}")]
       
        public ActionResult GetBookInfo(string moviename, string theatrename)
        {
            try
            {
                _logger.LogInformation($"Get ticket booking info : {moviename} at theatre : {theatrename}");

                var getBookedTickets = _ticketRepository.getBookedSeats(moviename, theatrename);
                var totalSeats = _ticketRepository.getTotalTickets(moviename, theatrename);

                return Ok(new
                {
                    totalSeatsAvailable = totalSeats,
                    bookedTickets = getBookedTickets,
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving booking info for movie : {moviename} at theatre : {theatrename}. Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Retrieves tickets associated with a user.
        /// </summary>
        /// <param name="loginId">The login ID of the user.</param>
        /// <returns>
        /// Returns tickets associated with the specified user.
        /// </returns>

        [Authorize(Roles = "Admin, Member")]
        [HttpGet]
        [Route("getticketsbyuser/{loginId}")]
       

        public async Task<ActionResult> GetTickets(string loginId)
        {
            try
            {
                _logger.LogInformation($"Getting all tickets : {loginId}");

                var tickets = await _ticketRepository.getTickets(loginId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while retrieving tickets for login ID: {loginId}. Error: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    }
}
