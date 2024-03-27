using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestAccura.Models;
using TestAccura.Repository;

namespace TestAccura.Controllers
{

    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IUserRepository userRepository, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHttpClientFactory httpClientFactory)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
            _httpClientFactory = httpClientFactory;

        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userRepository.GetAllUsersAsync();

                if (users == null || !users.Any())
                {
                    return NoContent(); // Return 204 No Content if no users are found
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it according to your application's needs
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("userList")]
        public async Task<IActionResult> GetAllUsersList(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Retrieve all courses from the repository.
                var users = await _userRepository.GetAllUsersAsync();

                // Paginate the course data based on pageNumber and pageSize.
                var paginatedUsers = PaginateData(users, pageNumber, pageSize);

                return Ok(paginatedUsers);
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex, "An error occurred while retrieving courses.");
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        private IEnumerable<User> PaginateData(IEnumerable<User> data, int pageNumber, int pageSize)
        {
            // Calculate the number of items to skip.
            var skip = (pageNumber - 1) * pageSize;

            // Apply pagination using LINQ's Skip and Take methods.
            var paginatedData = data.Skip(skip).Take(pageSize);

            return paginatedData;
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);

                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            try
            {
                var user = await _userRepository.AuthenticateAsync(model.Email, model.Password);

                if (user == null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Generate JWT token
                var token = GenerateJwtToken(user.Email);

                return Ok(new { Token = token, Email = model.Email, UserId = user.UserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] User request)
        {
            try
            {
                // Validate request
                if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                {
                    return BadRequest("Username and password are required.");
                }

                // Check if username already exists
                var existingUser = await _userRepository.AuthenticateExsitingUserAsync(request.Email, request.Password);
                if (existingUser != null)
                {
                    return Conflict("Username already exists.");
                }

                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                // Create user
                var newUser = new User
                {
                    Email = request.Email,
                    CreatedDate = DateTime.Now,
                };

                await _userRepository.RegisterAsync(newUser, request.Password);

                // Generate JWT token
                var token = GenerateJwtToken(newUser.Email);

                return Ok(new { Message = "User registered successfully.", Token = token });
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                return StatusCode(500, "Internal server error");
            }
        }






        private string GenerateJwtToken(string username)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                // Add any additional claims here
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _userRepository.GetUserById(id);
                if (user == null)
                {
                    //_logger.LogWarning($"Payment with ID {id} not found.");
                    return NotFound();
                }

                await _userRepository.DeleteUser(id);
                //_logger.LogInformation($"Deleted payment with ID {id}.");
                return NoContent(); // 204 No Content response
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An error occurred while deleting a payment.");
                return StatusCode(500, "An error occurred while processing your request.");
            }


        }

    

    }

}
