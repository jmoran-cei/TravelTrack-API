using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTrack_API.Migrations
{
    public partial class AddPhotosSetAndSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Photos",
                columns: new[] { "Id", "AddedByUser", "Alt", "FileName", "FileType", "Path", "TripId" },
                values: new object[] { new Guid("2a663dcd-60ac-409a-9ec8-14ac915aa1af"), "jmoran@ceiamerica.com", "1-travel-track-readme-img.jpg", "1-travel-track-readme-img.jpg", "image/jpg", "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-travel-track-readme-img.jpg", 1L });

            migrationBuilder.InsertData(
                table: "Photos",
                columns: new[] { "Id", "AddedByUser", "Alt", "FileName", "FileType", "Path", "TripId" },
                values: new object[] { new Guid("c25e28d6-7069-4aaa-b5a0-088fb46604a8"), "jmoran@ceiamerica.com", "1-sample-trip-img.jpg", "1-sample-trip-img.jpg", "image/jpg", "https://bootcampjmoranstorage.blob.core.windows.net/trip-photos/1-sample-trip-img.jpg", 1L });

            migrationBuilder.CreateIndex(
                name: "IX_Photos_TripId",
                table: "Photos",
                column: "TripId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Photos");
        }
    }
}
