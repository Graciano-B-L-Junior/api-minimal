using Microsoft.EntityFrameworkCore;
using Minimal.Entidades;
using Minimal.Infraestrutura.Db;
using Minimal.Servicos;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTeste
{
    private DbContexto CriarContextoTeste(){
        var options = new DbContextOptionsBuilder<DbContexto>()
        .UseNpgsql("Host=localhost:5433;Username=example;Password=example;Database=minimalapiteste").Options;
        return new DbContexto(options);
        
    }
    [TestMethod]
    public void TesteSalvarAdministrador()
    {
        //Arrange
        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "adm@teste.com";
        adm.Perfil = "Adm";
        adm.Senha ="123";

        var contexto = CriarContextoTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE \"Administradores\"");
        var administradorServico = new AdministradorServico(contexto);
        
        //Act
        administradorServico.IncluirAdministrador(adm);

        //Assert
        Assert.AreEqual(1,administradorServico.Todos(1).Count());
        Assert.AreEqual("adm@teste.com",adm.Email);
        Assert.AreEqual("Adm",adm.Perfil);
        Assert.AreEqual("123",adm.Senha);
    }
}