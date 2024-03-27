using System.Net.Mail;
using System.Net;
using TestAccura.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;

namespace TestAccura.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly AccuraContext _context;

        public UserRepository(AccuraContext context)
        {
            _context = context;
        }



        public async Task<User> AuthenticateAsync(string Email, string password)
        {
            // Implementation of user authentication logic goes here
            // This could involve querying a database, checking a password hash, etc.

            try
            {
                // Retrieve the user from the data store based on the username
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

                if (user == null)
                {
                    // User with the provided username not found
                    return null;
                }

                // Example: You may use a library like BCrypt to verify the password hash
                // Replace 'VerifyPassword' with your actual password verification logic
                bool isPasswordValid = VerifyPassword(password, user.Password);

                if (!isPasswordValid)
                {
                    // Password is incorrect
                    return null;
                }

                // Authentication successful; return the user object
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during authentication
                // Log the error or perform appropriate error handling
                throw;
            }
        }

        public async Task<User> AuthenticateExsitingUserAsync(string Email, string password)
        {
            // Implementation of user authentication logic goes here
            // This could involve querying a database, checking a password hash, etc.

            try
            {
                // Retrieve the user from the data store based on the username
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

                if (user == null)
                {
                    // User with the provided username not found
                    return null;
                }

                // Authentication successful; return the user object
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during authentication
                // Log the error or perform appropriate error handling
                throw;
            }
        }

        // Example password verification method (you can replace it with your actual logic)
        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            // Implement your password verification logic here
            // You may use a library like BCrypt to securely hash and verify passwords
            // For demonstration purposes, we'll use a simple comparison here
            return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
        }

        //public Task<User> RegisterAsync(User user, string password)
        //{
        //    throw new NotImplementedException();
        //}


        public async Task<User> RegisterAsync(User user, string password)
        {
            try
            {
                // Check if a user with the same username already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

                if (existingUser != null)
                {
                    // A user with the same username already exists; you may want to handle this case
                    // For this example, we'll return null, but you can throw an exception or handle it differently
                    return null;
                }

                // Securely hash the user's password before storing it
                string passwordHash = HashPassword(password);

                // Set the password hash for the user
                user.Password = passwordHash;

                // Add the user to the data store and save changes
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                // Return the newly registered user
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during registration
                // Log the error or perform appropriate error handling
                throw;
            }
        }

        // Example password hashing method (you should use a secure hashing library)
        private string HashPassword(string password)
        {
            // Implement your password hashing logic here (e.g., using BCrypt or Argon2)
            // For demonstration purposes, we'll use a simple hashing method here
            // Replace this with a secure hashing implementation
            // Generate a salt and hash the password with BCrypt
            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return hashedPassword;


        }

 

 

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetUserByEmailAsync(string email, int? RoleId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);

                return user; // Returns null if not found
            }
            catch (Exception ex)
            {
                // Handle exceptions, e.g., log or throw
                return null;
            }
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync(); // Assuming your user entity is called "Users"
        }

        public async Task DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

    

    }
}
