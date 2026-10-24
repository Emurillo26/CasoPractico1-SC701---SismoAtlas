// Punto de entrada de la app MVC de SismoAtlas (la única pantalla / landing page).

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// TODO (MVC): registrar el servicio (HttpClient) que llama al API de SismoAtlas
// usando "SismoAtlasApi:BaseUrl" del appsettings.json.

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
