using Apivscode2.Interfaces;
using Apivscode2.Models;
using Apivscode2.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apivscode2.Controllers;

[ApiController]
[Route("[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomersRepository _repository;
    public CustomersController(ICustomersRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Get()
    {
        var users = await _repository.SearchUsersAsync();
        return users.Any() ? Ok(users) : NoContent();
    }
    [HttpGet("id")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repository.SearchUserByIdAsync(id);
        return user != null ? Ok(user) : NotFound("Cliente não encontrado");
    }
    [HttpPost("customer")]
    [Authorize]
    public async Task<IActionResult> Post(CustomersRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Cpf))
            return BadRequest("CPF é obrigatório.");

        if (!CpfValidator.CpfIsValid(request.Cpf))
            return BadRequest("CPF inválido.");

        if (string.IsNullOrWhiteSpace(request.PublicPlace) ||
            string.IsNullOrWhiteSpace(request.Neighborhood) ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.State) ||
            string.IsNullOrWhiteSpace(request.Cep))
            return BadRequest("Endereço completo é obrigatório.");

        var duplicates = await _repository.CheckExistingCustomersAsync(request.Cpf);

        if (duplicates)
            return BadRequest("Este CPF já está cadastrado.");

        var isAdd = await _repository.AddCustomersAsync(request);

        return isAdd
            ? Ok("Cliente cadastrado com sucesso!")
            : BadRequest("Erro ao cadastrar Cliente.");
    }

    [HttpPut("id")]
    [Authorize]
    public async Task<IActionResult> Put([FromBody] CustomersRequestDTO request, int id)
    {
        if (id <= 0)
            return BadRequest("Cliente Inválido");

        var user = await _repository.SearchUserByIdAsync(id);
        if (user == null)
            return NotFound("Cliente não existe na base de dados");

        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Cpf))
            return BadRequest("CPF é obrigatório.");

        if (!CpfValidator.CpfIsValid(request.Cpf))
            return BadRequest("CPF inválido.");

        if (string.IsNullOrWhiteSpace(request.PublicPlace) ||
            string.IsNullOrWhiteSpace(request.Neighborhood) ||
            string.IsNullOrWhiteSpace(request.City) ||
            string.IsNullOrWhiteSpace(request.State) ||
            string.IsNullOrWhiteSpace(request.Cep))
            return BadRequest("Endereço completo é obrigatório.");

        var duplicates = await _repository.CheckExistingCpfForUpdateAsync(request.Cpf, id);

        if (duplicates)
            return BadRequest("Este CPF já está cadastrado por outro usuário.");

        var updated = await _repository.UpdateCustomer(request, id);

        return updated
            ? Ok("Cliente atualizado com sucesso!")
            : BadRequest("Erro ao atualizar Cliente");
    }

    [HttpDelete("id")]
    [Authorize]
    public async Task<IActionResult> Delete(int id) 
    {
        if(id <= 0) return BadRequest("Cliente não encontrado");
        var deleted = await _repository.DeleteCustomerAsync(id);
        return deleted 
            ? Ok("Cliente deletado com sucesso!")
            : BadRequest("Erro ao deletar Cliente"); 
    }
}
