using Licitaciones.Application.Comun;
using Licitaciones.Application.Interfaces;
using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.NivelesAprobacion;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.TipoCambio;
using Licitaciones.Domain.Excepciones;
using Licitaciones.Infrastructure.Persistencia;
using Licitaciones.Infrastructure.Repositorios;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API — Sistema de Gestión de Licitaciones",
        Version = "v1"
    });
});

builder.Services.AddDbContext<LicitacionesDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("LicitacionesDb")));

builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddScoped<IProveedorRepository, ProveedorRepository>();
builder.Services.AddScoped<ILicitacionRepository, LicitacionRepository>();
builder.Services.AddScoped<IOfertaRepository, OfertaRepository>();
builder.Services.AddScoped<INivelAprobacionRepository, NivelAprobacionRepository>();
builder.Services.AddScoped<ITipoCambioRepository, TipoCambioRepository>();

builder.Services.AddScoped<ProveedorAppService>();
builder.Services.AddScoped<LicitacionAppService>();
builder.Services.AddScoped<OfertaAppService>();
builder.Services.AddScoped<NivelAprobacionAppService>();
builder.Services.AddScoped<TipoCambioAppService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(o => o.RoutePrefix = "swagger");

// Manejo global de errores: nunca se expone el stack trace al cliente.
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var (status, codigo, mensaje) = feature?.Error switch
        {
            RecursoNoEncontradoException ex => (StatusCodes.Status404NotFound, "recurso.no_encontrado", ex.Message),
            OfertaDuplicadaException ex => (StatusCodes.Status409Conflict, ex.Codigo, ex.Message),
            DomainException ex => (StatusCodes.Status422UnprocessableEntity, ex.Codigo, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "error.interno", "Ocurrió un error inesperado.")
        };

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = codigo,
            Detail = mensaje
        });
    });
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
