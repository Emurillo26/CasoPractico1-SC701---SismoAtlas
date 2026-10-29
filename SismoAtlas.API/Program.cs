using Microsoft.EntityFrameworkCore;
using SismoAtlas.DAL.Context;
using SismoAtlas.DAL.Repositories;

// Punto de entrada del API de SismoAtlas.
// Aquí se registran los servicios (controllers, caché, EF, repositorios, servicios externos).

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Memory Cache (requisito 14): disponible para los servicios que llaman a USGS y RestCountries.
builder.Services.AddMemoryCache();

// DAL: conexión a SQL Server con Entity Framework Core
builder.Services.AddDbContext<SismoAtlasDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SismoAtlasDb")));

// DAL: Repository Pattern
builder.Services.AddScoped<IEarthquakeRepository, EarthquakeRepository>();

// TODO (API USGS - Jefry): registrar el servicio de carga inicial de sismos.
// TODO (API RestCountries - Dayana): registrar el servicio de países.

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();