﻿// <auto-generated />
using PSOBB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PSOBB.Database.GameServer.Migrations.CommonGameDatabase
{
    [DbContext(typeof(CommonGameDatabaseContext))]
    [Migration("20181109233142_PathWaypointsMigration")]
    partial class PathWaypointsMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Guardians.PathWaypointModel", b =>
                {
                    b.Property<int>("PathId");

                    b.Property<int>("PointId");

                    b.HasKey("PathId", "PointId");

                    b.ToTable("path_waypoints");
                });

            modelBuilder.Entity("Guardians.PathWaypointModel", b =>
                {
                    b.OwnsOne("Guardians.Database.Vector3<float>", "Point", b1 =>
                        {
                            b1.Property<int>("PathWaypointModelPathId");

                            b1.Property<int>("PathWaypointModelPointId");

                            b1.Property<float>("X");

                            b1.Property<float>("Y");

                            b1.Property<float>("Z");

                            b1.ToTable("path_waypoints");

                            b1.HasOne("Guardians.PathWaypointModel")
                                .WithOne("Point")
                                .HasForeignKey("Guardians.Database.Vector3<float>", "PathWaypointModelPathId", "PathWaypointModelPointId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
