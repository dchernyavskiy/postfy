using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postfy.Services.Network.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial002 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_messages_parent_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_user_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_users_messages_message_id",
                schema: "network",
                table: "users");

            migrationBuilder.DropIndex(
                name: "ix_users_message_id",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "message_id",
                schema: "network",
                table: "users");

            migrationBuilder.AddColumn<string>(
                name: "bio",
                schema: "network",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "notification_settings_someone_followed",
                schema: "network",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notification_settings_someone_liked_post",
                schema: "network",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notification_settings_someone_mentioned",
                schema: "network",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notification_settings_someone_send_follow_request",
                schema: "network",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "notification_settings_someone_send_message",
                schema: "network",
                table: "users",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "privacy_settings_who_can_follow_me",
                schema: "network",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "privacy_settings_who_can_message_me",
                schema: "network",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_messages_parent_id",
                schema: "network",
                table: "messages",
                column: "parent_id",
                principalSchema: "network",
                principalTable: "messages",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_sender_id",
                schema: "network",
                table: "messages",
                column: "sender_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_messages_parent_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_sender_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropColumn(
                name: "bio",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification_settings_someone_followed",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification_settings_someone_liked_post",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification_settings_someone_mentioned",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification_settings_someone_send_follow_request",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "notification_settings_someone_send_message",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "privacy_settings_who_can_follow_me",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "privacy_settings_who_can_message_me",
                schema: "network",
                table: "users");

            migrationBuilder.AddColumn<Guid>(
                name: "message_id",
                schema: "network",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_message_id",
                schema: "network",
                table: "users",
                column: "message_id");

            migrationBuilder.AddForeignKey(
                name: "fk_messages_messages_parent_id",
                schema: "network",
                table: "messages",
                column: "parent_id",
                principalSchema: "network",
                principalTable: "messages",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_messages_users_user_id",
                schema: "network",
                table: "messages",
                column: "sender_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_messages_message_id",
                schema: "network",
                table: "users",
                column: "message_id",
                principalSchema: "network",
                principalTable: "messages",
                principalColumn: "id");
        }
    }
}
