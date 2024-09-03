using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minimal.Dominios.enuns;
using Minimal.Dominios.ModelViews;
using Minimal.DTOs;
using Minimal.Entidades;
using Minimal.Infraestrutura.Db;
using Minimal.Interfaces;
using Minimal.Servicos;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option => {
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddSwaggerGen( options => {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
        Name= "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddDbContext<DbContexto>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAuthorization();

var app = builder.Build();

string GerarTokenJwt(Administrador administrador){
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

    var claims =new List<Claim>(){
        new Claim("Email",administrador.Email),
        new Claim("Perfil",administrador.Perfil),
        new Claim(ClaimTypes.Role, administrador.Perfil)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}


ErroValidacaoMensagem validaDTO(VeiculoDTO veiculoDTO)
{
    var mensagens = new ErroValidacaoMensagem { Mensagens = new List<string>()};

    if(string.IsNullOrEmpty(veiculoDTO.Nome)) mensagens.Mensagens.Add("Nome do veículo não pode ser nulo");

    if(string.IsNullOrEmpty(veiculoDTO.Marca)) mensagens.Mensagens.Add("Nome da marca não pode ser nulo");

    if(veiculoDTO.Ano < 1950) mensagens.Mensagens.Add("O sistema só aceita ano de fabricação de veículo superior ou igual a 1950");

    return mensagens;
}

app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    
    var adm =administradorServico.Login(loginDTO);

    if(adm != null){
        
        string token = GerarTokenJwt(adm);
        var adm_logado = new AdministradorLogado{
            Email = adm.Email,
            perfil = adm.Perfil,
            token = token
        };
        return Results.Ok(adm_logado);
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("Administradores");

app.MapGet("/administradores/{id}", ([FromQuery] int id, IAdministradorServico administradorServico) => {
    var adm = administradorServico.BuscarPorId(id);
    if (adm == null) return Results.NotFound();
    AdministradorModelView admView = new AdministradorModelView {
        Email = adm.Email,
        Id = adm.Id,
        perfil = adm.Perfil.ToString()
    };
    return Results.Ok(admView);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administradores");

app.MapGet("/administradores/", ([FromQuery] int pagina, IAdministradorServico administradorServico) => {
    var adms = administradorServico.Todos(pagina);
    List<AdministradorModelView> lista_adm = new List<AdministradorModelView>();
    foreach (var adm in adms){
        AdministradorModelView admView = new AdministradorModelView {
            Email = adm.Email,
            Id = adm.Id,
            perfil = adm.Perfil.ToString()
        };
        lista_adm.Add(admView);
    }
    return Results.Ok(lista_adm);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administradores");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico)=>{
    Administrador adm = new Administrador {
        Id = administradorDTO.Id,
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.perfil.ToString()
    };
    administradorServico.IncluirAdministrador(adm);
    return Results.Created($"/administradores/{adm.Id}",adm);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administradores");

app.MapGet("/veiculos",([FromQuery]int? pagina, IVeiculoServico veiculoServico)=>{
    var veiculos = veiculoServico.Todos(pagina,nome: "");

    return Results.Ok(veiculos);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor"})
.WithTags("Veiculos");

app.MapGet("/veiculos/{id}",([FromRoute]int id, IVeiculoServico veiculoServico)=>{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor"})
.WithTags("Veiculos");

app.MapPut("/veiculos/{id}",([FromRoute]int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=>{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Ano = veiculoDTO.Ano;
    veiculo.Marca = veiculoDTO.Marca;

    var mensagens = validaDTO(veiculoDTO);
    if(mensagens.Mensagens.Count > 0) return Results.BadRequest(mensagens.Mensagens);

    veiculoServico.AtualizarVeiculo(veiculo);

    return Results.Ok(veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Veiculos");

app.MapDelete("/veiculos/{id}",([FromRoute]int id, IVeiculoServico veiculoServico)=>{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null) return Results.NotFound();


    veiculoServico.RemoverVeiculo(veiculo);

    return Results.NoContent();
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Veiculos");

app.MapPost("/veiculo", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=>{
    Veiculo veiculo = new Veiculo {
        Nome = veiculoDTO.Nome,
        Ano = veiculoDTO.Ano,
        Marca = veiculoDTO.Marca
    };
    var mensagens = validaDTO(veiculoDTO);
    if(mensagens.Mensagens.Count > 0) return Results.BadRequest(mensagens.Mensagens);
    veiculoServico.IncluirVeiculo(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}",veiculo);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor"})
.WithTags("Veiculos");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
