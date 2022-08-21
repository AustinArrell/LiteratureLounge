﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Purrs_And_Prose.Data;

#nullable disable

namespace LiteratureLounge.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20220817012030_TakeOffNullableFields2")]
    partial class TakeOffNullableFields2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("LiteratureLounge.Models.Book", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("ChapterLength")
                        .HasColumnType("longtext");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MediaType")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int?>("Rating")
                        .HasColumnType("int");

                    b.Property<string>("ReadStatus")
                        .HasColumnType("longtext");

                    b.Property<string>("Synopsis")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("isAnnotated")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isCheckedOut")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isSigned")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isStamped")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });
#pragma warning restore 612, 618
        }
    }
}
