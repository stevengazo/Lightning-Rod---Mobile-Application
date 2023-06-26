using Manchito.Model;
using Microsoft.EntityFrameworkCore;

namespace Manchito.DataBaseContext
{
    public class DBLocalContext : DbContext
    {

        public DbSet<AudioNote> AudioNote { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<ItemType> ItemTypes { get; set; }
        public DbSet<Maintenance> Maintenance { get; set; }
        public DbSet<Note> Note { get; set; }
        public DbSet<Photography> Photography { get; set; }
        public DbSet<Project> Project { get; set; }


        public DBLocalContext()
        {
            SQLitePCL.Batteries_V2.Init();
            this.Database.EnsureCreated();

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string PathDatabase = Path.Combine(FileSystem.CacheDirectory, "mydatabase.db");
            optionsBuilder.UseSqlite($"Filename={PathDatabase}");


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            List<ItemType> itemTypes = new() {
                new()
                {
                    ItemTypeId= 1,
                    Name="Zona Cobertura"
                },
                new()
                {
                    ItemTypeId= 2,
                    Name="Montaje"
                },
                new()
                {
                    ItemTypeId= 3,
                    Name="Pararrayos"
                },
                new()
                {
                    ItemTypeId= 4,
                    Name="Bajante"
                },
                new()
                {
                    ItemTypeId= 5,
                    Name="Sistema Puesta a Tierra"
                },
                new()
                {
                    ItemTypeId= 6,
                    Name="Unificaciones"
                },
                new()
                {
                    ItemTypeId= 7,
                    Name="Supresores"
                },
                new()
                {
                    ItemTypeId= 8,
                    Name="Otros"
                },
            };
            modelBuilder.Entity<ItemType>().HasData(itemTypes);
        }
    }
}
