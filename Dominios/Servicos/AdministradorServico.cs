
using Minimal.DTOs;
using Minimal.Entidades;
using Minimal.Infraestrutura.Db;
using Minimal.Interfaces;

namespace Minimal.Servicos;
public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto){
        _contexto = contexto;
    }
    public Boolean Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where<Administrador>( a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).Count();
        return adm > 0;
    }
}