using Apivscode2.DTOs;
using Apivscode2.Interfaces;
using Apivscode2.Models;
using Apivscode2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Apivscode2.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersRepository _repository;
    public UsersController(IUsersRepository repository)
    {
        _repository = repository;
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO login)
    {
        var user = await _repository.Authenticate(login.Email, login.Password);

        if (user == null)
            return Unauthorized(new { message = "Usuário ou senha inválidos" });

        var token = TokenServices.GenerateToken(user);
        var refreshToken = TokenServices.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        var updated = await _repository.UpdateUserAsync(user);
        if (!updated)
            return BadRequest(new { message = "Erro ao gerar refresh token." });

        return Ok(new
        {
            user = user.Name,
            id = user.Id,
            token = token,
            refreshToken = refreshToken
        });
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest("Refresh token é obrigatório.");

        var user = await _repository.GetUserByRefreshTokenAsync(request.RefreshToken);

        if (user == null)
            return Unauthorized(new { message = "Refresh token inválido." });

        var newToken = TokenServices.GenerateToken(user);

        var newRefreshToken = TokenServices.GenerateRefreshToken();
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _repository.UpdateUserAsync(user);

        return Ok(new
        {
            token = newToken,
            refreshToken = newRefreshToken
        });
    }
    [HttpPost("user")]
    public async Task<IActionResult> Post(UsersRequestDTO request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Senha é obrigatória.");

        var duplicates = await _repository.CheckExistingUserEmailAsync(request.Email);

        if (duplicates)
            return BadRequest("Este e-mail já está cadastrado.");

        request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var isAdd = await _repository.AddUserAsync(request);

        return isAdd
            ? Ok("Usuário cadastrado com sucesso!")
            : BadRequest("Erro ao cadastrar usuário.");
    }
}
