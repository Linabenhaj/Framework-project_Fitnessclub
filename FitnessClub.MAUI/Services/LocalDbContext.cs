using FitnessClub.MAUI.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessClub.MAUI.Data
{
    public class LocalDbContext : DbContext
    {
        public DbSet<LocalUser> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source=fitnessclub.db");
        }
    }
}