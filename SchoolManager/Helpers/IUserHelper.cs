using Microsoft.AspNetCore.Identity;
using SchoolManager.Data.Entities;

namespace SchoolManager.Helpers
{
    public interface IUserHelper
    {
        Task<User?> GetUserByEmailAsync(string email);

        Task<IdentityResult> AddUserAsync(User user, string password);

        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);

        Task LogoutAsync();

        Task<bool> CheckRoleAsync(string roleName);

        Task AddRoleAsync(string roleName);

        Task AddUserToRoleAsync(User user, string roleName);

        Task<bool> IsUserInRoleAsync(User user, string roleName);

        Task<IdentityResult> UpdateUserAsync(User user);

        Task<IdentityResult> ChangePasswordAsync(
            User user,
            string oldPassword,
            string newPassword);

        Task<string> GeneratePasswordResetTokenAsync(User user);

        Task<IdentityResult> ResetPasswordAsync(
            User user,
            string token,
            string newPassword);
    }
}
