using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postfy.Services.Network.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "network");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "chats",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chats", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "chat_user",
                schema: "network",
                columns: table => new
                {
                    chatid = table.Column<Guid>(name: "chat_id", type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_chat_user", x => new { x.chatid, x.userid });
                    table.ForeignKey(
                        name: "fk_chat_user_chats_chat_id",
                        column: x => x.chatid,
                        principalSchema: "network",
                        principalTable: "chats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comments",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    parentid = table.Column<Guid>(name: "parent_id", type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_comments", x => x.id);
                    table.ForeignKey(
                        name: "fk_comments_comments_parent_id",
                        column: x => x.parentid,
                        principalSchema: "network",
                        principalTable: "comments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "follower_following",
                schema: "network",
                columns: table => new
                {
                    followerid = table.Column<Guid>(name: "follower_id", type: "uuid", nullable: false),
                    followingid = table.Column<Guid>(name: "following_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_follower_following", x => new { x.followerid, x.followingid });
                });

            migrationBuilder.CreateTable(
                name: "message_medias",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: true),
                    messageid = table.Column<Guid>(name: "message_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_medias", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "messages",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    text = table.Column<string>(type: "text", nullable: false),
                    ispost = table.Column<bool>(name: "is_post", type: "boolean", nullable: false),
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: true),
                    senderid = table.Column<Guid>(name: "sender_id", type: "uuid", nullable: false),
                    parentid = table.Column<Guid>(name: "parent_id", type: "uuid", nullable: true),
                    chatid = table.Column<Guid>(name: "chat_id", type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_messages", x => x.id);
                    table.ForeignKey(
                        name: "fk_messages_chats_chat_id",
                        column: x => x.chatid,
                        principalSchema: "network",
                        principalTable: "chats",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_messages_messages_parent_id",
                        column: x => x.parentid,
                        principalSchema: "network",
                        principalTable: "messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    firstname = table.Column<string>(name: "first_name", type: "character varying(100)", maxLength: 100, nullable: false),
                    lastname = table.Column<string>(name: "last_name", type: "character varying(100)", maxLength: 100, nullable: false),
                    profilename = table.Column<string>(name: "profile_name", type: "character varying(100)", maxLength: 100, nullable: false),
                    profileimageid = table.Column<Guid>(name: "profile_image_id", type: "uuid", nullable: true),
                    profileimageurl = table.Column<string>(name: "profile_image_url", type: "text", nullable: true),
                    profileimagetype = table.Column<string>(name: "profile_image_type", type: "text", nullable: true),
                    profileimageposition = table.Column<int>(name: "profile_image_position", type: "integer", nullable: true),
                    signupdate = table.Column<DateTime>(name: "signup_date", type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    messageid = table.Column<Guid>(name: "message_id", type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_messages_message_id",
                        column: x => x.messageid,
                        principalSchema: "network",
                        principalTable: "messages",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "posts",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    caption = table.Column<string>(type: "text", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    messageid = table.Column<Guid>(name: "message_id", type: "uuid", nullable: true),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_posts", x => x.id);
                    table.ForeignKey(
                        name: "fk_posts_messages_message_id",
                        column: x => x.messageid,
                        principalSchema: "network",
                        principalTable: "messages",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_posts_users_user_id",
                        column: x => x.userid,
                        principalSchema: "network",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_medias",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<string>(type: "text", nullable: true),
                    position = table.Column<int>(type: "integer", nullable: true),
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_medias", x => x.id);
                    table.ForeignKey(
                        name: "fk_post_medias_posts_post_id",
                        column: x => x.postid,
                        principalSchema: "network",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "post_user",
                schema: "network",
                columns: table => new
                {
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_post_user", x => new { x.postid, x.userid });
                    table.ForeignKey(
                        name: "fk_post_user_posts_post_id",
                        column: x => x.postid,
                        principalSchema: "network",
                        principalTable: "posts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_post_user_users_user_id",
                        column: x => x.userid,
                        principalSchema: "network",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "reactions",
                schema: "network",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isliked = table.Column<bool>(name: "is_liked", type: "boolean", nullable: false),
                    userid = table.Column<Guid>(name: "user_id", type: "uuid", nullable: false),
                    postid = table.Column<Guid>(name: "post_id", type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    createdby = table.Column<int>(name: "created_by", type: "integer", nullable: true),
                    originalversion = table.Column<long>(name: "original_version", type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reactions", x => x.id);
                    table.ForeignKey(
                        name: "fk_reactions_posts_post_id",
                        column: x => x.postid,
                        principalSchema: "network",
                        principalTable: "posts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_reactions_users_user_id",
                        column: x => x.userid,
                        principalSchema: "network",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_chat_user_user_id",
                schema: "network",
                table: "chat_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_chats_id",
                schema: "network",
                table: "chats",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_comments_id",
                schema: "network",
                table: "comments",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_comments_parent_id",
                schema: "network",
                table: "comments",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_post_id",
                schema: "network",
                table: "comments",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_comments_user_id",
                schema: "network",
                table: "comments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_follower_following_following_id",
                schema: "network",
                table: "follower_following",
                column: "following_id");

            migrationBuilder.CreateIndex(
                name: "ix_message_medias_id",
                schema: "network",
                table: "message_medias",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_message_medias_message_id",
                schema: "network",
                table: "message_medias",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_chat_id",
                schema: "network",
                table: "messages",
                column: "chat_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_id",
                schema: "network",
                table: "messages",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_messages_parent_id",
                schema: "network",
                table: "messages",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "ix_messages_sender_id",
                schema: "network",
                table: "messages",
                column: "sender_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_id",
                schema: "network",
                table: "post_medias",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_post_medias_post_id",
                schema: "network",
                table: "post_medias",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_post_user_user_id",
                schema: "network",
                table: "post_user",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_posts_id",
                schema: "network",
                table: "posts",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_posts_message_id",
                schema: "network",
                table: "posts",
                column: "message_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_posts_user_id",
                schema: "network",
                table: "posts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_reactions_id",
                schema: "network",
                table: "reactions",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reactions_post_id",
                schema: "network",
                table: "reactions",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "ix_reactions_user_id",
                schema: "network",
                table: "reactions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_id",
                schema: "network",
                table: "users",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_message_id",
                schema: "network",
                table: "users",
                column: "message_id");

            migrationBuilder.AddForeignKey(
                name: "fk_chat_user_users_user_id",
                schema: "network",
                table: "chat_user",
                column: "user_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_posts_post_id",
                schema: "network",
                table: "comments",
                column: "post_id",
                principalSchema: "network",
                principalTable: "posts",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_comments_users_user_id",
                schema: "network",
                table: "comments",
                column: "user_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_follower_following_users_follower_id",
                schema: "network",
                table: "follower_following",
                column: "follower_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_follower_following_users_following_id",
                schema: "network",
                table: "follower_following",
                column: "following_id",
                principalSchema: "network",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_message_medias_messages_message_id",
                schema: "network",
                table: "message_medias",
                column: "message_id",
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_messages_chats_chat_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropForeignKey(
                name: "fk_messages_users_user_id",
                schema: "network",
                table: "messages");

            migrationBuilder.DropTable(
                name: "chat_user",
                schema: "network");

            migrationBuilder.DropTable(
                name: "comments",
                schema: "network");

            migrationBuilder.DropTable(
                name: "follower_following",
                schema: "network");

            migrationBuilder.DropTable(
                name: "message_medias",
                schema: "network");

            migrationBuilder.DropTable(
                name: "post_medias",
                schema: "network");

            migrationBuilder.DropTable(
                name: "post_user",
                schema: "network");

            migrationBuilder.DropTable(
                name: "reactions",
                schema: "network");

            migrationBuilder.DropTable(
                name: "posts",
                schema: "network");

            migrationBuilder.DropTable(
                name: "chats",
                schema: "network");

            migrationBuilder.DropTable(
                name: "users",
                schema: "network");

            migrationBuilder.DropTable(
                name: "messages",
                schema: "network");
        }
    }
}
