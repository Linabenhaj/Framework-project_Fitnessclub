using FitnessClub.Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace FitnessClub.Models.Data
{
    public interface IFitnessClubDbContext
    {
        DbSet<Gebruiker> Gebruikers { get; set; }
        DbSet<Abonnement> Abonnementen { get; set; }
        DbSet<Les> Lessen { get; set; }
        DbSet<Inschrijving> Inschrijvingen { get; set; }

        DatabaseFacade Database { get; }
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        // Voeg deze methodes toe:
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        EntityEntry Entry(object entity);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        int SaveChanges();
    }
}