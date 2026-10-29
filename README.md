# SismoAtlas

Aplicación web para explorar la actividad sísmica de un país. Consulta los sismos registrados por el **USGS** (magnitud 3 o más, desde 2020), los guarda en una base de datos SQL Server y, al buscar un país, muestra sus sismos junto con información del país obtenida de **REST Countries**.

Caso Práctico 1 — Programación Avanzada Web (SC-701) — Grupo 4.

## Integrantes

- Rivas Galagarza Álvaro Alexander
- Goleath Arrieta Jefry
- Gómez Sequeira María Pamela
- García Murillo Engel
- Castillo Villalobos Dayana

## Estructura de la solución

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `SismoAtlas.API` | ASP.NET Core Web API | Controllers, servicios de USGS y REST Countries, Memory Cache |
| `SismoAtlas.DAL` | Class Library | Entity Framework Core, entidades, DbContext y Repository Pattern |
| `SismoAtlas.DTO` | Class Library | DTOs que viajan entre capas (ej. `EarthquakeDto`) |
| `SismoAtlas.MVC` | ASP.NET Core MVC | Landing page con buscador, servicio que consume el API |
| `Database/` | Scripts SQL | Script de creación de la base de datos |

## Requisitos

- .NET 10 SDK
- Visual Studio 2022 (17.14+) o Visual Studio 2026
- SQL Server (local o Express)

## Cómo ejecutar

1. Ejecutar el script de `Database/` en SQL Server para crear la base de datos.
2. Revisar en `SismoAtlas.API/appsettings.json`:
   - `ConnectionStrings:SismoAtlasDb` (cadena de conexión a su SQL Server).
   - `ExternalApis:RestCountries:ApiKey` (API key de REST Countries).
3. En Visual Studio: clic derecho en la solución → **Configure Startup Projects** → **Multiple startup projects** → poner `SismoAtlas.API` y `SismoAtlas.MVC` en **Start**.
4. Ejecutar (F5). El API corre en `https://localhost:7100` y la app en `https://localhost:7200`.

## Funcionalidades

- Carga inicial de sismos desde USGS a la base de datos (sin volver a llamar si ya existen, salvo registros nuevos).
- Búsqueda de sismos por nombre de país.
- Información del país desde REST Countries en la misma respuesta.
- Memory Cache en las llamadas a los API externos.
- Validaciones de entrada y errores personalizados con SweetAlert2.

## Adicionales

_Por completar._

## Tecnologías

ASP.NET Core 10, Entity Framework Core, SQL Server, IMemoryCache, SweetAlert2, HTML/CSS responsivo.
