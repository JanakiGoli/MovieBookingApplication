using AccessIdentityAPI.DTOS;
using AccessIdentityAPI.Interfaces;
using AccessIdentityAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace AccessIdentityAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        protected ILogger<AuthenticationController> _logger;


        public AuthenticationController(UserManager<ApplicationUser> userManager, ITokenService tokenService, ILogger<AuthenticationController> logger)
        {
            this._userManager = userManager;
            this._tokenService = tokenService;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user with the system.
        /// </summary>
        /// <param name="request">The registration request containing user information.</param>
        /// <returns>
        /// An asynchronous task representing the registration operation. Upon completion, returns an IActionResult indicating the result of the registration attempt:
        /// - 400 Bad Request if the email is already registered or the login ID already exists.
        /// - 200 OK with a success message if registration is successful.
        /// </returns>

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                _logger.LogInformation($"User Registern with  : {request.LoginId}");

                if (await _userManager.FindByEmailAsync(request.Email) != null)
                {
                    return BadRequest("Email is already registered");
                }
                if (await _userManager.FindByNameAsync(request.LoginId) != null)
                {
                    return BadRequest("LoginId already exists");
                }
                ApplicationUser appUser = new ApplicationUser
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    UserName = request.LoginId,
                    PhoneNumber = request.ContactNumber
                };

                appUser.SecurityStamp = Guid.NewGuid().ToString();
                await _userManager.CreateAsync(appUser, request.Password);
                await _userManager.AddToRoleAsync(appUser, "Member");
                return Ok("Registration Successfull");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during registration: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Logs in a user with the provided login credentials.
        /// </summary>
        /// <param name="request">The login request containing user login credentials.</param>
        /// <returns>
        /// An asynchronous task representing the login operation. Upon completion, returns an IActionResult indicating the result of the login attempt:
        /// - 400 Bad Request if the provided login ID or password is invalid.
        /// - 200 OK with a JSON object containing the user's login ID and a JWT token if login is successful.
        /// </returns>

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation($"Logging in user with login id : {request.LoginId}");


                bool check = false;
                var user = await _userManager.FindByNameAsync(request.LoginId);
                if (user != null)
                {
                    check = await _userManager.CheckPasswordAsync(user, request.Password);
                }
                if (user == null || !check)
                {
                    return BadRequest("Invalid LoginId/Password");
                }
                var token = await _tokenService.CreateToken(user);
                return Ok(new { loginId = user.UserName, jwtToken = token });
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while logging in: {ex.Message}");
                return StatusCode(500, "Internal server error");

            }
        }

        /// <summary>
        /// Resets the password for a user who has forgotten their password.
        /// </summary>
        /// <param name="request">The request containing the user's login ID and the new password.</param>
        /// <returns>
        /// - 400 Bad Request if the user does not exist.
        /// - 200 OK with a success message if the password is reset successfully.
        /// </returns>


        [HttpPut]
        [Route("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                _logger.LogInformation($"Reset password for user with login id : {request.LoginId}");

                var user = await _userManager.FindByNameAsync(request.LoginId);
                if (user == null)
                {
                    return BadRequest("User doesn't exists");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, request.NewPassword);
                return Ok("Password updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred while resetting password: {ex.Message}");
                return StatusCode(500, "Internal server error");

            }
        }

    }
}



