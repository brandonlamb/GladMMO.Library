﻿// <auto-generated />
using Guardians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace Guardians.Database.GameServer.Migrations
{
    [DbContext(typeof(CharacterDatabaseContext))]
    partial class CharacterDatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("Guardians.CharacterEntryModel", b =>
                {
                    b.Property<int>("CharacterId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<string>("CharacterName")
                        .IsRequired();

                    b.Property<DateTime>("CreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

                    b.HasKey("CharacterId");

                    b.HasAlternateKey("CharacterName");

                    b.ToTable("characters");
                });

            modelBuilder.Entity("Guardians.CharacterSessionModel", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccountId");

                    b.Property<int>("CharacterId");

                    b.Property<DateTime>("SessionCreationDate")
                        .ValueGeneratedOnAddOrUpdate()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

                    b.Property<int>("ZoneId");

                    b.HasKey("SessionId");

                    b.HasAlternateKey("AccountId");


                    b.HasAlternateKey("CharacterId");

                    b.HasIndex("ZoneId")
                        .IsUnique();

                    b.ToTable("character_sessions");
                });

            modelBuilder.Entity("Guardians.ZoneInstanceEntryModel", b =>
                {
                    b.Property<int>("ZoneId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ZoneServerAddress")
                        .IsRequired();

                    b.Property<short>("ZoneServerPort");

                    b.Property<int>("ZoneType");

                    b.HasKey("ZoneId");

                    b.ToTable("zone_endpoints");
                });

            modelBuilder.Entity("Guardians.CharacterSessionModel", b =>
                {
                    b.HasOne("Guardians.CharacterEntryModel", "CharacterEntry")
                        .WithOne()
                        .HasForeignKey("Guardians.CharacterSessionModel", "CharacterId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Guardians.ZoneInstanceEntryModel", "ZoneEntry")
                        .WithOne()
                        .HasForeignKey("Guardians.CharacterSessionModel", "ZoneId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
