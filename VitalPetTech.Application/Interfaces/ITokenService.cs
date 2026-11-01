using VitalPetTech.Application.DTOs;
using VitalPetTech.Domain.Entities;

namespace VitalPetTech.Application.Interfaces;

public interface ITokenService
{
    TokenResponse Gerar(Usuario usuario);
}