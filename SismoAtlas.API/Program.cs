// Punto de entrada del API de SismoAtlas.
// Aquí se registran los servicios (controllers, caché, EF, repositorios, servicios externos).

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Memory Cache (requisito 14): disponible para los servicios que llaman a USGS y RestCountries.
builder.Services.AddMemoryCache();

// TODO (DAL): registrar el DbContext y los repositorios.
// TODO (API USGS): registrar el servicio de carga inicial de sismos.
// TODO (API RestCountries): registrar el servicio de países.

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
