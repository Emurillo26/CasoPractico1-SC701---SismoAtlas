
using SismoAtlas.MVC.Services;
using System;

// Punto de entrada de la app MVC de SismoAtlas (la única pantalla / landing page).

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// MVC: registrar el cliente HTTP que se comunicará con SismoAtlas.API.
// La dirección se obtiene desde appsettings.json.
builder.Services.AddHttpClient("SismoAtlasApi", client =>
{
    // Obtener la URL base de nuestra API.
    var baseUrl = builder.Configuration["SismoAtlasApi:BaseUrl"];

    // Verificar que la dirección esté configurada correctamente.
    if (string.IsNullOrWhiteSpace(baseUrl) ||
        !Uri.TryCreate(baseUrl, UriKind.Absolute, out var apiUri))
    {
        throw new InvalidOperationException(
            "La dirección de SismoAtlas.API no está configurada correctamente.");
    }

    // Configurar la dirección y el tiempo máximo de espera.
    client.BaseAddress = apiUri;
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Registrar el servicio que consultará los terremotos y países.
// Este servicio utilizará el HttpClient configurado anteriormente.
builder.Services.AddScoped<SismoAtlasApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
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
