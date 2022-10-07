using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTrack_API.Migrations
{
    public partial class seedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Destination",
                columns: new[] { "Id", "City", "Country", "Region" },
                values: new object[,]
                {
                    { "ChIJASFVO5VoAIkRGJbQtRWxD7w", "Myrtle Beach", "United States", "South Carolina" },
                    { "ChIJdySo3EJ6_ogRa-zhruD3-jU", "Charleston", "United States", "South Carolina" },
                    { "ChIJw4OtEaZjDowRZCw_jCcczqI", "Zemi Beach", "Anguilla", "West End" }
                });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "TripId", "Details", "EndDate", "ImgURL", "StartDate", "Title" },
                values: new object[,]
                {
                    { 1L, "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.", new DateTime(2022, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "assets/images/trips/anguila1.jpg", new DateTime(2022, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brothers' Anguila Trip" },
                    { 2L, "Oremlay ipsumyay olorday itsay ametyay, onsectetuercay adipiscingyay elityay. Edsay itaevay eolay inyay iamday empersay orttitorpay. Ullamnay idyay augueyay. Aecenasmay atyay acuslay isquay islnay auctoryay imperdietyay. Integeryay incidunttay acinialay elitvay. Uspendissesay aretraphay. Uisday ariusvay. Ellentesquepay abitanthay orbimay istiquetray enectussay etyay etusnay etyay alesuadamay amesfay acyay urpistay egestasyay.", new DateTime(2022, 6, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "assets/images/trips/myrtlebeach1.jpg", new DateTime(2022, 5, 27, 0, 0, 0, 0, DateTimeKind.Unspecified), "Myrtle Beach and Charleston Family Vacay 2022" },
                    { 3L, "", new DateTime(2024, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "assets/images/trips/myrtlebeach1.jpg", new DateTime(2024, 7, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Another Test Trip" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Username", "FirstName", "LastName", "Password" },
                values: new object[,]
                {
                    { "dummyuser@dummy.dum", "Dummy", "User", "P@ssw0rd" },
                    { "fakeyfake@fakey.fake", "Fake", "User", "P@ssw0rd" },
                    { "jmoran@ceiamerica.com", "Jonathan", "Moran", "P@ssw0rd" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Destination",
                keyColumn: "Id",
                keyValue: "ChIJASFVO5VoAIkRGJbQtRWxD7w");

            migrationBuilder.DeleteData(
                table: "Destination",
                keyColumn: "Id",
                keyValue: "ChIJdySo3EJ6_ogRa-zhruD3-jU");

            migrationBuilder.DeleteData(
                table: "Destination",
                keyColumn: "Id",
                keyValue: "ChIJw4OtEaZjDowRZCw_jCcczqI");

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Trips",
                keyColumn: "TripId",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "dummyuser@dummy.dum");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "fakeyfake@fakey.fake");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Username",
                keyValue: "jmoran@ceiamerica.com");
        }
    }
}
