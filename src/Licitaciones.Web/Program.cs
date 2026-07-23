using Licitaciones.Application.Licitaciones;
using Licitaciones.Application.NivelesAprobacion;
using Licitaciones.Application.Ofertas;
using Licitaciones.Application.Proveedores;
using Licitaciones.Application.TipoCambio;
using Licitaciones.Application.Interfaces;
using Licitaciones.Infrastructure.Persistencia;
using Licitaciones.Infrastructure.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
