using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Entities.Migrations
{
    public partial class GetPerson_StoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                CREATE PROCEDURE GetAllPersons
                AS BEGIN
                    SELECT * from Persons;
                END
                        
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string sp_GetAllPersons = @"
                Drop PROCEDURE GetAllPersons                
                        
            ";
            migrationBuilder.Sql(sp_GetAllPersons);
        }
    }
}
