using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTrack_API.Migrations
{
    public partial class CompleteRenewedDBMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "B2CUser",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B2CUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Destinations",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Destinations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImgURL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Username);
                });

            migrationBuilder.CreateTable(
                name: "Photos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TripId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Photos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Photos_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ToDo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Task = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Complete = table.Column<bool>(type: "bit", nullable: false),
                    TripId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDo_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripB2CMembers",
                columns: table => new
                {
                    TripId = table.Column<long>(type: "bigint", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripB2CMembers", x => new { x.TripId, x.UserId });
                    table.ForeignKey(
                        name: "FK_TripB2CMembers_B2CUser_UserId",
                        column: x => x.UserId,
                        principalTable: "B2CUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripB2CMembers_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripDestinations",
                columns: table => new
                {
                    TripId = table.Column<long>(type: "bigint", nullable: false),
                    DestinationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripDestinations", x => new { x.TripId, x.DestinationId });
                    table.ForeignKey(
                        name: "FK_TripDestinations_Destinations_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Destinations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripDestinations_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TripMembers",
                columns: table => new
                {
                    TripId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripMembers", x => new { x.TripId, x.Username });
                    table.ForeignKey(
                        name: "FK_TripMembers_Trips_TripId",
                        column: x => x.TripId,
                        principalTable: "Trips",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TripMembers_Users_Username",
                        column: x => x.Username,
                        principalTable: "Users",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "B2CUser",
                columns: new[] { "Id", "Username" },
                values: new object[,]
                {
                    { "3cf84869-e3b3-4ff4-9421-0c5271e6cc92", "jmoran@ceiamerica.com" },
                    { "7241d571-59ec-40f7-8730-84de1ff982d6", "testuser@test.test" }
                });

            migrationBuilder.InsertData(
                table: "Destinations",
                columns: new[] { "Id", "City", "Country", "Region" },
                values: new object[,]
                {
                    { "ChIJASFVO5VoAIkRGJbQtRWxD7w", "Myrtle Beach", "United States", "South Carolina" },
                    { "ChIJdySo3EJ6_ogRa-zhruD3-jU", "Charleston", "United States", "South Carolina" },
                    { "ChIJw4OtEaZjDowRZCw_jCcczqI", "Zemi Beach", "Anguilla", "West End" }
                });

            migrationBuilder.InsertData(
                table: "Trips",
                columns: new[] { "Id", "Details", "EndDate", "ImgURL", "StartDate", "Title" },
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
                    { "fakeuser@fakey.fake", "Fake", "User", "P@ssw0rd" },
                    { "jmoran@ceiamerica.com", "Jonathan", "Moran", "P@ssw0rd" }
                });

            migrationBuilder.InsertData(
                table: "Photos",
                columns: new[] { "Id", "AddedByUser", "Alt", "FileName", "FileType", "Path", "TripId" },
                values: new object[,]
                {
                    { new Guid("11223344-5566-7788-99aa-bbccddeeff00"), "jmoran@ceiamerica.com", "1-sample-trip-img.jpg", "1-sample-trip-img.jpg", "image/jpg", "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-sample-trip-img.jpg", 1L },
                    { new Guid("21223344-5566-7788-99aa-bbccddeeff00"), "jmoran@ceiamerica.com", "1-travel-track-readme-img.jpg", "1-travel-track-readme-img.jpg", "image/jpg", "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-travel-track-readme-img.jpg", 1L }
                });

            migrationBuilder.InsertData(
                table: "ToDo",
                columns: new[] { "Id", "Complete", "Task", "TripId" },
                values: new object[,]
                {
                    { 1, true, "buy new swim trunks", 1L },
                    { 2, true, "pack beach towels", 1L },
                    { 3, true, "buy new swim trunks", 2L },
                    { 4, true, "buy new swim trunks", 2L }
                });

            migrationBuilder.InsertData(
                table: "TripB2CMembers",
                columns: new[] { "TripId", "UserId" },
                values: new object[,]
                {
                    { 1L, "3cf84869-e3b3-4ff4-9421-0c5271e6cc92" },
                    { 1L, "7241d571-59ec-40f7-8730-84de1ff982d6" },
                    { 2L, "3cf84869-e3b3-4ff4-9421-0c5271e6cc92" },
                    { 2L, "7241d571-59ec-40f7-8730-84de1ff982d6" },
                    { 3L, "3cf84869-e3b3-4ff4-9421-0c5271e6cc92" }
                });

            migrationBuilder.InsertData(
                table: "TripDestinations",
                columns: new[] { "DestinationId", "TripId" },
                values: new object[,]
                {
                    { "ChIJw4OtEaZjDowRZCw_jCcczqI", 1L },
                    { "ChIJASFVO5VoAIkRGJbQtRWxD7w", 2L },
                    { "ChIJdySo3EJ6_ogRa-zhruD3-jU", 2L },
                    { "ChIJASFVO5VoAIkRGJbQtRWxD7w", 3L },
                    { "ChIJdySo3EJ6_ogRa-zhruD3-jU", 3L }
                });

            migrationBuilder.InsertData(
                table: "TripMembers",
                columns: new[] { "TripId", "Username" },
                values: new object[,]
                {
                    { 1L, "dummyuser@dummy.dum" },
                    { 1L, "jmoran@ceiamerica.com" },
                    { 2L, "dummyuser@dummy.dum" },
                    { 2L, "fakeuser@fakey.fake" },
                    { 2L, "jmoran@ceiamerica.com" },
                    { 3L, "jmoran@ceiamerica.com" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_TripId",
                table: "Photos",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_ToDo_TripId",
                table: "ToDo",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripB2CMembers_UserId",
                table: "TripB2CMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TripDestinations_DestinationId",
                table: "TripDestinations",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_TripMembers_Username",
                table: "TripMembers",
                column: "Username");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");

            migrationBuilder.DropTable(
                name: "ToDo");

            migrationBuilder.DropTable(
                name: "TripB2CMembers");

            migrationBuilder.DropTable(
                name: "TripDestinations");

            migrationBuilder.DropTable(
                name: "TripMembers");

            migrationBuilder.DropTable(
                name: "B2CUser");

            migrationBuilder.DropTable(
                name: "Destinations");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
