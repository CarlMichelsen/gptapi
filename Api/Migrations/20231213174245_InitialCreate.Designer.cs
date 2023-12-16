﻿// <auto-generated />
using System;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20231213174245_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entity.Conversation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastAppended")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<bool>("UserDeleted")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserProfileId");

                    b.ToTable("Conversation");
                });

            modelBuilder.Entity("Domain.Entity.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<bool>("Complete")
                        .HasColumnType("boolean");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ResponseId")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<bool>("Visible")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.ToTable("Message");
                });

            modelBuilder.Entity("Domain.Entity.OAuthRecord", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AccessToken")
                        .HasColumnType("text");

                    b.Property<int>("AuthenticationMethod")
                        .HasColumnType("integer");

                    b.Property<string>("Error")
                        .HasColumnType("text");

                    b.Property<DateTime>("RedirectedToThirdParty")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("ReturnedFromThirdParty")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<Guid?>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserProfileId");

                    b.ToTable("OAuthRecord");
                });

            modelBuilder.Entity("Domain.Entity.UserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<string>("AuthenticationId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("AuthenticationIdType")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthenticationId")
                        .IsUnique();

                    b.ToTable("UserProfile");
                });

            modelBuilder.Entity("Domain.Entity.Conversation", b =>
                {
                    b.HasOne("Domain.Entity.UserProfile", "UserProfile")
                        .WithMany()
                        .HasForeignKey("UserProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("UserProfile");
                });

            modelBuilder.Entity("Domain.Entity.Message", b =>
                {
                    b.HasOne("Domain.Entity.Conversation", null)
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");
                });

            modelBuilder.Entity("Domain.Entity.OAuthRecord", b =>
                {
                    b.HasOne("Domain.Entity.UserProfile", null)
                        .WithMany("OAuthRecords")
                        .HasForeignKey("UserProfileId");
                });

            modelBuilder.Entity("Domain.Entity.Conversation", b =>
                {
                    b.Navigation("Messages");
                });

            modelBuilder.Entity("Domain.Entity.UserProfile", b =>
                {
                    b.Navigation("OAuthRecords");
                });
#pragma warning restore 612, 618
        }
    }
}