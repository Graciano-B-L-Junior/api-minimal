
using Minimal.Dominios.enuns;

public class AdministradorDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    
    public string Senha { get; set; }
    public Perfil perfil { get; set; }
}