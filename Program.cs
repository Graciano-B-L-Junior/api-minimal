using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minimal.Dominios.ModelViews;
using Minimal.DTOs;
using Minimal.Entidades;
using Minimal.Infraestrutura.Db;
using Minimal.Interfaces;
using Minimal.Servicos;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddDbContext<DbContexto>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();



app.MapGet("/", () => Results.Json(new Home()));

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) => {
    if(administradorServico.Login(loginDTO))
        return Results.Ok("Login com sucesso");
    else
        return Results.Unauthorized();
});

app.MapGet("/veiculos",([FromQuery]int? pagina, IVeiculoServico veiculoServico)=>{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
});

app.MapPost("/veiculo", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico)=>{
    Veiculo veiculo = new Veiculo {
        Nome = veiculoDTO.Nome,
        Ano = veiculoDTO.Ano,
        Marca = veiculoDTO.Marca
    };
    veiculoServico.IncluirVeiculo(veiculo);
    return Results.Created($"/veiculos/{veiculo.Id}",veiculo);
});

app.UseSwagger();
app.UseSwaggerUI();

app.Run();
