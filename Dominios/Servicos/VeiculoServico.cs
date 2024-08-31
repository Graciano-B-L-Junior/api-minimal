
using Minimal.Entidades;
using Minimal.Infraestrutura.Db;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    public void AtualizarVeiculo(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public Veiculo BuscarPorId(int Id)
    {
        Veiculo veiculo = _contexto.Veiculos.Find(Id)!;
        return veiculo;
    }

    public void IncluirVeiculo(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public void RemoverVeiculo(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        int itensPorPagina = 10;
        var query = _contexto.Veiculos;
        List<Veiculo> result = new List<Veiculo>();
        if(nome != null){

            result = query.Where(v => v.Nome.ToLower().Contains(nome)).Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina).ToList()!;
        }
        return result;
    }
}