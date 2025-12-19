using Apivscode2.Interfaces;
using Apivscode2.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;


namespace Apivscode2.Repository
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public UsersRepository(IConfiguration configuration){
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("Default");
        }
        public async Task<UsersRequestDTO?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            string sql = @"
                SELECT id, name, email, password, refresh_token AS RefreshToken, refresh_token_expiry AS RefreshTokenExpiryTime
                FROM tb_users
                WHERE refresh_token = @RefreshToken
            ";

            using var con = new NpgsqlConnection(connectionString);
            var user = await con.QueryFirstOrDefaultAsync<UsersRequestDTO>(sql, new
            {
                RefreshToken = refreshToken,
                Now = DateTime.UtcNow
            });

            return user;
        }
        public async Task<bool> AddUserAsync(UsersRequestDTO request)
        {
            string sql = @"
                INSERT INTO public.tb_users
                (name, email, password)
                VALUES
                (@Name, @Email, @Password);
            ";

            using var con = new NpgsqlConnection(connectionString);

            var rows = await con.ExecuteAsync(sql, new
            {
                request.Name,
                request.Email,
                request.Password
            });

            return rows > 0;
        }
        public async Task<UsersRequestDTO?> Authenticate(string email, string password)
        {
            string sql = @"
                SELECT id, name, email, password
                FROM tb_users
                WHERE email = @Email
            ";

            using var con = new NpgsqlConnection(connectionString);
            var user = await con.QueryFirstOrDefaultAsync<UsersRequestDTO>(sql, new { Email = email });

            if (user == null)
                return null;

            bool passwordOk = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return passwordOk ? user : null;
        }
        public async Task<bool> UpdateUserAsync(UsersRequestDTO user)
        {
            string sql = @"
                UPDATE tb_users
                SET refresh_token = @RefreshToken,
                    refresh_token_expiry = @RefreshTokenExpiryTime
                WHERE id = @Id
            ";

            using var con = new NpgsqlConnection(connectionString);
            var rows = await con.ExecuteAsync(sql, new
            {
                user.RefreshToken,
                user.RefreshTokenExpiryTime,
                user.Id
            });

            return rows > 0;
        }
        public async Task<bool> CheckExistingUserEmailAsync(string email)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM tb_users
                WHERE email = @Email
            ";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                int count = await connection.ExecuteScalarAsync<int>(query, new { Email = email });
                return count > 0;
            }
        }
    }
}