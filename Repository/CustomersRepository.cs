using Apivscode2.Interfaces;
using Apivscode2.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace Apivscode2.Repository
{
    public class CustomersRepository : ICustomersRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string connectionString;
        public CustomersRepository(IConfiguration configuration){
            _configuration = configuration;
            connectionString = _configuration.GetConnectionString("Default");
        }
        public async Task<IEnumerable<CustomersResponseDTO>> SearchUsersAsync()
        {
            string sql = @"
                SELECT 
                    id,
                    name,
                    cpf,
                    public_place AS publicPlace,
                    neighborhood,
                    city,
                    state,
                    cep
                FROM public.tb_customers;
            ";

            using var con = new NpgsqlConnection(connectionString);
            return await con.QueryAsync<CustomersResponseDTO>(sql);
        }

        public async Task<CustomersResponseDTO> SearchUserByIdAsync(int id)
        {
            string sql = @"
                SELECT
                    id,
                    name,
                    cpf,
                    public_place AS publicPlace,
                    neighborhood,
                    city,
                    state,
                    cep
                FROM public.tb_customers
                WHERE id = @Id;
            ";

            using var con = new NpgsqlConnection(connectionString);
            return await con.QueryFirstOrDefaultAsync<CustomersResponseDTO>(sql, new { Id = id });
        }
        public async Task<bool> UpdateCustomer(CustomersRequestDTO request, int id)
        {
            string sql = @"
                UPDATE public.tb_customers
                SET
                    name = @Name,
                    cpf = @Cpf,
                    public_place = @PublicPlace,
                    neighborhood = @Neighborhood,
                    city = @City,
                    state = @State,
                    cep = @Cep
                WHERE id = @Id;
            ";

            using var con = new NpgsqlConnection(connectionString);

            var rows = await con.ExecuteAsync(sql, new
            {
                request.Name,
                request.Cpf,
                request.PublicPlace,
                request.Neighborhood,
                request.City,
                request.State,
                request.Cep,
                Id = id
            });

            return rows > 0;
        }
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            string sql = @"DELETE FROM public.tb_customers WHERE id = @Id";

            using var con = new NpgsqlConnection(connectionString);

            var rows = await con.ExecuteAsync(sql, new { Id = id });

            return rows > 0;
        }
        public async Task<bool> AddCustomersAsync(CustomersRequestDTO request)
        {
            string sql = @"
                INSERT INTO public.tb_customers
                (name, cpf, public_place, neighborhood, city, state, cep)
                VALUES
                (@Name, @Cpf, @PublicPlace, @Neighborhood, @City, @State, @Cep);
            ";

            using var con = new NpgsqlConnection(connectionString);

            var rows = await con.ExecuteAsync(sql, new
            {
                request.Name,
                request.Cpf,
                request.PublicPlace,
                request.Neighborhood,
                request.City,
                request.State,
                request.Cep 

            });

            return rows > 0;
        }
        public async Task<bool> CheckExistingCustomersAsync(string cpf)
        {
            var query = @"
                SELECT COUNT(*) 
                FROM tb_customers
                WHERE cpf = @Cpf
            ";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                int count = await connection.ExecuteScalarAsync<int>(query, new { Cpf = cpf });
                return count > 0;
            }
        }

        public async Task<bool> CheckExistingCpfForUpdateAsync(string cpf, int id)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM tb_customers
                WHERE cpf = @Cpf AND id <> @Id;
            ";

            using var con = new NpgsqlConnection(connectionString);

            int count = await con.ExecuteScalarAsync<int>(sql, new { Cpf = cpf, Id = id });

            return count > 0;
        }
    }
}