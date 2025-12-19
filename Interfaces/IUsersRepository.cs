using Apivscode2.Models;

namespace Apivscode2.Interfaces
{
    public interface IUsersRepository
    {
        Task<UsersRequestDTO> Authenticate(string email, string password);
        Task<bool> AddUserAsync(UsersRequestDTO request);
        Task<bool> CheckExistingUserEmailAsync(string email);
        Task<UsersRequestDTO?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateUserAsync(UsersRequestDTO user);
    }
}