using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Person> Persons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Person>().ToTable("Persons");

            //seed to countries
            string countriesJson = System.IO.File.ReadAllText("countries.json");

            List<Country> countries = System.Text.Json.JsonSerializer.Deserialize<List<Country>>(countriesJson);
            foreach (Country country in countries)
            {
                modelBuilder.Entity<Country>().HasData(country);

            }

            //seed to Persons
            string personjson = System.IO.File.ReadAllText("persons.json");

            List<Person> persons = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(personjson);
            foreach (Person person in persons)
            {
                modelBuilder.Entity<Person>().HasData(person);

            }

            //fluent API configuration
            //we use it to upate column anem/data type/add default value/
            //add constarint  like unique or check or add index
            modelBuilder.Entity<Person>()
                .Property(p => p.TaxIdentificationNumber)
                .HasColumnName("TaxIdentificationNumber")
                .HasColumnType("nvarchar(8)")
                .HasDefaultValue("12keshav1234");

            //modelBuilder.Entity<Person>()
            //    .HasIndex(temp => temp.Tin).IsUnique();


            modelBuilder.Entity<Person>()
                .HasCheckConstraint("CHK_TIN", "len([TaxIdentificationNumber])=8");  

                //model relation -->another way to define relation between two entities
                // but we ae doing in entity class
                //modelBuilder.Entity<Person>(entity =>
                //{
                //    entity.HasOne<Country>(p => p.Country)
                //        .WithMany(c => c.Persons)
                //        .HasForeignKey(p => p.CountryID);
                //});

        }

        public List<Person> sp_GetAllPersons()
        {
            //This method is not used in the current implementation, but can be used to execute a stored procedure to get all persons
            //You can implement this method if you want to use stored procedures in your application
            //Example: return this.Database.ExecuteSqlRaw("EXEC sp_GetAllPersons");

            return Persons.FromSqlRaw("EXEC GetAllPersons").ToList();
        }

        public int sp_InsertPerson(Person person)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@PesonID", person.PersonID),
                new SqlParameter("@PersonName", person.PersonName ?? (object)DBNull.Value),
                new SqlParameter("@Email", person.Email ?? (object)DBNull.Value),
                new SqlParameter("@DateOfBirth", person.DateOfBirth ?? (object)DBNull.Value),
                new SqlParameter("@Gender", person.Gender ) ,
                new SqlParameter("@CountryId", person.CountryID ?? (object)DBNull.Value),
                new SqlParameter("@Address", person.Address ?? (object)DBNull.Value),
                new SqlParameter("@ReceiveNewsLetters", person.ReceiveNewsLetters)

            };
           return Database.ExecuteSqlRaw("EXEC InsertPerson @PesonID, @PersonName, @Email, @DateOfBirth, @Gender, @CountryId, @Address, @ReceiveNewsLetters", parameters);
        }
    }
}
