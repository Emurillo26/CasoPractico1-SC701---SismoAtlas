using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SismoAtlas.API.Middleware;
using SismoAtlas.API.Services;
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

// API RestCountries (Dayana)
builder.Services.Configure<RestCountriesOptions>(
    builder.Configuration.GetSection(RestCountriesOptions.SectionName));

builder.Services.AddHttpClient<ICountryService, CountryService>((sp, client) =>
{
    var opts = sp.GetRequiredService<IOptions<RestCountriesOptions>>().Value;

    var baseUrl = opts.BaseUrl.EndsWith('/') ? opts.BaseUrl : opts.BaseUrl + "/";
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(opts.TimeoutSeconds);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

    if (!string.IsNullOrWhiteSpace(opts.ApiKey))
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", opts.ApiKey);
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();