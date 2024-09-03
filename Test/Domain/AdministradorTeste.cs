using Minimal.Entidades;

namespace Test.Domain;

[TestClass]
public class AdministradorTeste
{
    [TestMethod]
    public void TesteGetSetPropriedade()
    {
        //Arrange
        var adm = new Administrador();
        //Act
        adm.Id = 1;
        adm.Email = "adm@teste.com";
        adm.Perfil = "Adm";
        adm.Senha ="123";
        //Assert
        Assert.AreEqual(1,adm.Id);
        Assert.AreEqual("adm@teste.com",adm.Email);
        Assert.AreEqual("Adm",adm.Perfil);
        Assert.AreEqual("123",adm.Senha);
    }
}