using TestAccura.Models;

namespace TestAccura.Repository
{
    public interface IUserRepository
    {
        Task<User> AuthenticateAsync(string Email, string Password);
        Task<User> RegisterAsync(User user, string Password);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserById(int id);
        Task<IEnumerable<User>> GetAllUsersAsync(); // New method to fetch all users
        Task<User> AuthenticateExsitingUserAsync(string Email, string Password);
        Task DeleteUser(int id);

    }
}
