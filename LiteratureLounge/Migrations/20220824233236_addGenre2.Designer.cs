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
    [Migration("20220824233236_addGenre2")]
    partial class addGenre2
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

                    b.Property<string>("CheckedOutTo")
                        .HasColumnType("longtext");

                    b.Property<string>("CoverLink")
                        .HasColumnType("longtext");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MediaType")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<float?>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("ReadStatus")
                        .HasColumnType("longtext");

                    b.Property<string>("SignatureType")
                        .HasColumnType("longtext");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("isAnnotated")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isCheckedOut")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isFavorite")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isSigned")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("isStamped")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("LiteratureLounge.Models.Genre", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Genres");
                });
#pragma warning restore 612, 618
        }
    }
}
