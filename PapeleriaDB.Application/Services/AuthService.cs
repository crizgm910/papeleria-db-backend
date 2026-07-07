using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PapeleriaDB.Application.DTOs;
using PapeleriaDB.Domain.Interfaces;

namespace PapeleriaDB.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;

    public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var usuario = await _unitOfWork.Usuarios.GetByUsernameAsync(dto.Username);

        if (usuario == null)
        {
            return new AuthResponseDto { Exito = false, Mensaje = "Usuario o contraseña incorrectos." };
        }

        bool esValido = BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash);

        if (!esValido)
        {
            return new AuthResponseDto { Exito = false, Mensaje = "Usuario o contraseña incorrectos." };
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"] ?? "SuperSecretaClaveLargaParaJWTPapeleria2026!!");
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, usuario.Rol)
            }),
            Expires = DateTime.UtcNow.AddHours(8),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new AuthResponseDto
        {
            Exito = true,
            Mensaje = "Login exitoso",
            Token = tokenHandler.WriteToken(token),
            NombreCompleto = usuario.NombreCompleto,
            Rol = usuario.Rol
        };
    }
}
