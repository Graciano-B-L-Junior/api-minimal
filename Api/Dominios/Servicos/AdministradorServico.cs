
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

    public Administrador? BuscarPorId(int Id)
    {
        var adm =_contexto.Administradores.Find(Id);
        return adm;
    }

    public Administrador IncluirAdministrador(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public Administrador Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where<Administrador>( a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).First();
        return adm;
    }

    public List<Administrador> Todos(int? pagina)
    {
        int itensPorPagina = 10;
        var query = _contexto.Administradores;
        List<Administrador> result = new List<Administrador>();

        result = query.Where(v => v.Email.Contains("")).Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina).ToList()!;
        return result;
    }
}