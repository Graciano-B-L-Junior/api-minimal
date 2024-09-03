
using Minimal.DTOs;
using Minimal.Entidades;

namespace Minimal.Interfaces;
public interface IAdministradorServico {
    Administrador Login(LoginDTO loginDTO);
    Administrador? BuscarPorId(int Id);
    List<Administrador> Todos(int? pagina);
    Administrador IncluirAdministrador(Administrador administrador);
}