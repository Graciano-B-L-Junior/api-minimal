
using Minimal.DTOs;
using Minimal.Entidades;

namespace Minimal.Interfaces;
public interface IAdministradorServico {
    Boolean Login(LoginDTO loginDTO);
}