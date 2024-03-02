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
    [Migration("20240203015106_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("GptApi")
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entity.Conversation", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("LastAppendedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Summary")
                        .HasColumnType("text");

                    b.Property<bool>("UserArchived")
                        .HasColumnType("boolean");

                    b.Property<Guid>("UserProfileId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.ToTable("Conversation", "GptApi");
                });

            modelBuilder.Entity("Domain.Entity.Message", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CompletedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Content")
                        .HasColumnType("text");

                    b.Property<Guid?>("ConversationId")
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedUtc")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("PreviousMessageId")
                        .HasColumnType("uuid");

                    b.Property<string>("ResponseId")
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<bool>("Visible")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("PreviousMessageId");

                    b.ToTable("Message", "GptApi");
                });

            modelBuilder.Entity("Domain.Entity.Message", b =>
                {
                    b.HasOne("Domain.Entity.Conversation", null)
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId");

                    b.HasOne("Domain.Entity.Message", "PreviousMessage")
                        .WithMany()
                        .HasForeignKey("PreviousMessageId");

                    b.Navigation("PreviousMessage");
                });

            modelBuilder.Entity("Domain.Entity.Conversation", b =>
                {
                    b.Navigation("Messages");
                });
#pragma warning restore 612, 618
        }
    }
}