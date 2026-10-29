using Microsoft.EntityFrameworkCore;
using SismoAtlas.DAL.Entities;

namespace SismoAtlas.DAL.Context
{
    // Puente entre C# y la base de datos SismoAtlasDB
    public class SismoAtlasDbContext : DbContext
    {
        public SismoAtlasDbContext(DbContextOptions<SismoAtlasDbContext> options)
            : base(options)
        {
        }

        // Representa la tabla dbo.Earthquakes
        public DbSet<Earthquake> Earthquakes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Los mismos índices que creamos en el script SQL
            modelBuilder.Entity<Earthquake>()
                .HasIndex(e => e.Country)
                .HasDatabaseName("IX_Earthquakes_Country");

            modelBuilder.Entity<Earthquake>()
                .HasIndex(e => e.Time)
                .HasDatabaseName("IX_Earthquakes_Time");
        }
    }
}