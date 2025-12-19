using Apivscode2.Models;

namespace Apivscode2.Interfaces
{
    public interface ICustomersRepository
    {
        Task<IEnumerable<CustomersResponseDTO>> SearchUsersAsync();
        Task<CustomersResponseDTO> SearchUserByIdAsync(int id);
        Task<bool> AddCustomersAsync(CustomersRequestDTO request);
        Task<bool> UpdateCustomer(CustomersRequestDTO request, int id);
        Task<bool> DeleteCustomerAsync(int id);
        Task<bool> CheckExistingCpfForUpdateAsync(string cpf, int id);
        Task<bool> CheckExistingCustomersAsync(string cpf);
        
    }
}