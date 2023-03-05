﻿// <auto-generated />
using System;
using Crash.Server.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Crash.Server.Migrations
{
    [DbContext(typeof(CrashContext))]
    [Migration("20221105223503_initial_database")]
    partial class initial_database
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("Crash.Server.Model.Change", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("LockedBy")
                        .HasColumnType("TEXT");

                    b.Property<string>("Owner")
                        .HasColumnType("TEXT");

                    b.Property<string>("Payload")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Stamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("Temporary")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Changes");
                });
#pragma warning restore 612, 618
        }
    }
}
