
using Microsoft.EntityFrameworkCore;
using Minimal.Entidades;

namespace Minimal.Infraestrutura.Db;
public class DbContexto : DbContext{
    public DbContexto(DbContextOptions<DbContexto> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            }
        );
    }
    public DbSet<Administrador> Administradores {get; set;} = default!;
    public DbSet<Veiculo> Veiculos {get; set;} = default!;
}