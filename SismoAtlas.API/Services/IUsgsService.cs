namespace SismoAtlas.API.Services
{

    public interface IUsgsService
    {

        Task CargaInicialAsync();


        Task<int> CargarNuevosAsync();
    }
}
