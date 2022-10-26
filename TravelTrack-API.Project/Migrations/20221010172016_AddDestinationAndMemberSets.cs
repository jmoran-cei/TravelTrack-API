using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TravelTrack_API.Migrations
{
    public partial class AddDestinationAndMemberSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDestination_Destination_DestinationId",
                table: "TripDestination");

            migrationBuilder.DropForeignKey(
                name: "FK_TripDestination_Trips_TripId",
                table: "TripDestination");

            migrationBuilder.DropForeignKey(
                name: "FK_TripUser_Trips_TripId",
                table: "TripUser");

            migrationBuilder.DropForeignKey(
                name: "FK_TripUser_Users_Username",
                table: "TripUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripUser",
                table: "TripUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripDestination",
                table: "TripDestination");

            migrationBuilder.RenameTable(
                name: "TripUser",
                newName: "TripUsers");

            migrationBuilder.RenameTable(
                name: "TripDestination",
                newName: "TripDestinations");

            migrationBuilder.RenameIndex(
                name: "IX_TripUser_Username",
                table: "TripUsers",
                newName: "IX_TripUsers_Username");

            migrationBuilder.RenameIndex(
                name: "IX_TripDestination_DestinationId",
                table: "TripDestinations",
                newName: "IX_TripDestinations_DestinationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripUsers",
                table: "TripUsers",
                columns: new[] { "TripId", "Username" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripDestinations",
                table: "TripDestinations",
                columns: new[] { "TripId", "DestinationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TripDestinations_Destination_DestinationId",
                table: "TripDestinations",
                column: "DestinationId",
                principalTable: "Destination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripDestinations_Trips_TripId",
                table: "TripDestinations",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripUsers_Trips_TripId",
                table: "TripUsers",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripUsers_Users_Username",
                table: "TripUsers",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripDestinations_Destination_DestinationId",
                table: "TripDestinations");

            migrationBuilder.DropForeignKey(
                name: "FK_TripDestinations_Trips_TripId",
                table: "TripDestinations");

            migrationBuilder.DropForeignKey(
                name: "FK_TripUsers_Trips_TripId",
                table: "TripUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_TripUsers_Users_Username",
                table: "TripUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripUsers",
                table: "TripUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TripDestinations",
                table: "TripDestinations");

            migrationBuilder.RenameTable(
                name: "TripUsers",
                newName: "TripUser");

            migrationBuilder.RenameTable(
                name: "TripDestinations",
                newName: "TripDestination");

            migrationBuilder.RenameIndex(
                name: "IX_TripUsers_Username",
                table: "TripUser",
                newName: "IX_TripUser_Username");

            migrationBuilder.RenameIndex(
                name: "IX_TripDestinations_DestinationId",
                table: "TripDestination",
                newName: "IX_TripDestination_DestinationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripUser",
                table: "TripUser",
                columns: new[] { "TripId", "Username" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_TripDestination",
                table: "TripDestination",
                columns: new[] { "TripId", "DestinationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_TripDestination_Destination_DestinationId",
                table: "TripDestination",
                column: "DestinationId",
                principalTable: "Destination",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripDestination_Trips_TripId",
                table: "TripDestination",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripUser_Trips_TripId",
                table: "TripUser",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripUser_Users_Username",
                table: "TripUser",
                column: "Username",
                principalTable: "Users",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
