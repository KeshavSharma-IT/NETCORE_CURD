using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class InsertPerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPersons = @"
                CREATE PROCEDURE InsertPerson
                (@PesonID uniqueidentifier,
                @PersonName nvarchar(40),
                @Email nvarchar(50),
                @DateOfBirth datetime2(7),
                @Gender nvarchar(10),
                @CountryId uniqueidentifier,
                @Address nvarchar(100), 
                @ReceiveNewsLetters bit               
                )
                AS BEGIN
                   Insert into Persons (PersonID,PersonName,Email,DateOfBirth,Gender,CountryID,Address,ReceiveNewsLetters) 
                     values (@PesonID,@PersonName,@Email,@DateOfBirth,@Gender,@CountryId,@Address,@ReceiveNewsLetters)
                END
                        
            ";
            migrationBuilder.Sql(sp_InsertPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_InsertPersons = @"
                Drop PROCEDURE sp_InsertPersons                
                        
            ";
            migrationBuilder.Sql(sp_InsertPersons);
        }
    }
}
