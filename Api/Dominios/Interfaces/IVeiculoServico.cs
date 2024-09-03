
using Minimal.Entidades;

public interface IVeiculoServico
{
    List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null);

    Veiculo BuscarPorId(int Id);
    
    void AtualizarVeiculo(Veiculo veiculo);
    void RemoverVeiculo(Veiculo veiculo);
    
    void IncluirVeiculo(Veiculo veiculo);
}