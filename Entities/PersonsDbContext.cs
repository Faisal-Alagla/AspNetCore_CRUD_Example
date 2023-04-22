﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace Entities
{
	public class PersonsDbContext : DbContext
	{
		public PersonsDbContext(DbContextOptions options) : base(options)
		{
		}

		public DbSet<Country> Countries { get; set; }
		public DbSet<Person> Persons { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Country>().ToTable("Countries");
			modelBuilder.Entity<Person>().ToTable("Persons");

			//Seed Data...
			//countries
			string countriesJson = System.IO.File.ReadAllText("countries.json");
			List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
			foreach (Country country in countries)
			{
				modelBuilder.Entity<Country>().HasData(country);
			}
			//persons
			string personsJson = System.IO.File.ReadAllText("persons.json");
			List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personsJson);
			foreach (Person person in persons)
			{
				modelBuilder.Entity<Person>().HasData(person);
			}
		}

		public List<Person> sp_GetAllPersons()
		{
			//IQueryable<Person>
			return Persons.FromSqlRaw("EXECUTE [dbo].[GetAllPersons]").ToList();
		}
	}
}
