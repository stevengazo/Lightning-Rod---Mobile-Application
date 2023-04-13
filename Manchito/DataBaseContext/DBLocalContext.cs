using Manchito.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manchito.DataBaseContext
{
	public class DBLocalContext: DbContext 
	{

		public DbSet<AudioNote> AudioNote { get; set; }	
		public DbSet<Category> Category { get; set; }
		public DbSet<ItemType> ItemTypes { get; set; }
		public DbSet<Maintenance> Maintenance { get; set; }
		public DbSet<Note> Note { get; set; }
		public DbSet<Photography> Photography { get; set; }
		public DbSet<Project> Project { get; set; }


		public DBLocalContext() { 
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
			Project _Project = new() {
				ProjectId = 1,
				Name = "Sample Name",
				CustomerContactName = "Customer Contact Name",
				CustomerName = "Customer Name"
			};
			modelBuilder.Entity<Project>().HasData(_Project);

			Maintenance _Maintenance = new() { 
				MaintenanceId = 1,
				DateOfMaintenance = DateTime.Now,
				Alias = "Alias Sample Site",
				Status = "In progress",
				ProjectId= _Project.ProjectId
			};
			modelBuilder.Entity<Maintenance>().HasData(_Maintenance);
			List<ItemType> itemTypes = new() {
				new()
				{
					ItemTypeId= 1,
					Name="Pararrayos"
				},
				new()
				{
					ItemTypeId= 2,
					Name="Montaje"
				},
				new()
				{
					ItemTypeId= 3,
					Name="Bajante"
				},
				new()
				{
					ItemTypeId= 4,
					Name="Sistema Puesta a Tierra"
				},
				new()
				{
					ItemTypeId= 5,
					Name="Unificaciones"
				},
				new()
				{
					ItemTypeId= 6,
					Name="Supresores"
				},
			};
			modelBuilder.Entity<ItemType>().HasData(itemTypes);			
		}
	}
}
