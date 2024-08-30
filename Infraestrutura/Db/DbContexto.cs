
using Microsoft.EntityFrameworkCore;
using Minimal.Entidades;

namespace Minimal.Infraestrutura.Db;
public class DbContexto : DbContext{
    private readonly IConfiguration _configurationAppSettings;
    public DbContexto(IConfiguration configurationAppSettings)
    {
        _configurationAppSettings = configurationAppSettings;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var conexaoString = _configurationAppSettings.GetConnectionString("mysql");
        optionsBuilder.UseMySql(
            "Server=localhost;Port=3306;Database=minimal_api;Uid=root;Pwd=example;",
            ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=minimal_api;Uid=root;Pwd=example;")
        );
    }
    public DbSet<Administrador> Administradores {get; set;} = default!;
}