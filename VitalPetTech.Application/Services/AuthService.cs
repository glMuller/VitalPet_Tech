using Microsoft.Extensions.Logging;
using VitalPetTech.Application.DTOs;
using VitalPetTech.Application.Exceptions;
using VitalPetTech.Application.Interfaces;
using VitalPetTech.Domain.Entities;
using VitalPetTech.Domain.Interfaces;

namespace VitalPetTech.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _repository;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUsuarioRepository repository,
        IPasswordHasher hasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _repository = repository;
        _hasher = hasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistrarRequest request)
    {
        var email = Normalizar(request.Email);

        if (await _repository.GetByEmailAsync(email) is not null)
            throw new BusinessRuleException("Já existe um usuário cadastrado com este e-mail.");

        // Regra de bootstrap: o primeiro usuário do sistema é o administrador
        var primeiroUsuario = !await _repository.AnyAsync();

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = _hasher.GerarHash(request.Senha),
            Perfil = primeiroUsuario ? PerfilUsuario.Admin : PerfilUsuario.Usuario,
            CriadoEm = DateTime.UtcNow
        };

        await _repository.AddAsync(usuario);
        _logger.LogInformation("Usuário {UsuarioId} registrado com o perfil {Perfil}.", usuario.Id, usuario.Perfil);

        return new UsuarioResponse(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil);
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        var usuario = await _repository.GetByEmailAsync(Normalizar(request.Email));

        // Mesma mensagem para e-mail inexistente e senha errada, para não revelar quais e-mails existem
        if (usuario is null || !_hasher.Verificar(request.Senha, usuario.SenhaHash))
        {
            _logger.LogWarning("Tentativa de login com credenciais inválidas.");
            throw new InvalidCredentialsException("E-mail ou senha inválidos.");
        }

        _logger.LogInformation("Usuário {UsuarioId} autenticado.", usuario.Id);
        return _tokenService.Gerar(usuario);
    }

    private static string Normalizar(string email) => email.Trim().ToLowerInvariant();
}