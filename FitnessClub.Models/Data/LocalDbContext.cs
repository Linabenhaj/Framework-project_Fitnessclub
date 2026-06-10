using Microsoft.EntityFrameworkCore;
using FitnessClub.Models.Models;

namespace FitnessClub.Models.Data
{
    // SQLite-gebaseerde lokale DbContext voor offline cache in de MAUI-app.
    // Bevindt zich in de Class Library zodat de modellen samen blijven met hun context
  
    public class LocalDbContext : DbContext
    {
        public LocalDbContext(DbContextOptions<LocalDbContext> options) : base(options)
        {
        }

        public DbSet<LocalUser> Users { get; set; }
        public DbSet<LocalLes> Lessen { get; set; }
        public DbSet<LocalInschrijving> Inschrijvingen { get; set; }
    }
}
