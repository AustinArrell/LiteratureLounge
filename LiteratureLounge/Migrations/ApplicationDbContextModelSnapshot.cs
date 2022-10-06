﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Purrs_And_Prose.Data;

#nullable disable

namespace LiteratureLounge.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.9")
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

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("MediaType")
                        .HasColumnType("longtext");

                    b.Property<string>("Notes")
                        .HasColumnType("longtext");

                    b.Property<int?>("PageCount")
                        .HasColumnType("int");

                    b.Property<string>("PublishedDate")
                        .HasColumnType("longtext");

                    b.Property<string>("Publisher")
                        .HasColumnType("longtext");

                    b.Property<float?>("Rating")
                        .HasColumnType("float");

                    b.Property<string>("ReadStatus")
                        .HasColumnType("longtext");

                    b.Property<string>("SignatureType")
                        .HasColumnType("longtext");

                    b.Property<string>("Subtitle")
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

            modelBuilder.Entity("LiteratureLounge.Models.BookGenre", b =>
                {
                    b.Property<int>("BookId")
                        .HasColumnType("int");

                    b.Property<int>("GenreId")
                        .HasColumnType("int");

                    b.HasKey("BookId", "GenreId");

                    b.HasIndex("GenreId");

                    b.ToTable("BookGenres");
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

            modelBuilder.Entity("LiteratureLounge.Models.BookGenre", b =>
                {
                    b.HasOne("LiteratureLounge.Models.Book", "Book")
                        .WithMany("BookGenres")
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("LiteratureLounge.Models.Genre", "Genre")
                        .WithMany("BookGenres")
                        .HasForeignKey("GenreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Genre");
                });

            modelBuilder.Entity("LiteratureLounge.Models.Book", b =>
                {
                    b.Navigation("BookGenres");
                });

            modelBuilder.Entity("LiteratureLounge.Models.Genre", b =>
                {
                    b.Navigation("BookGenres");
                });
#pragma warning restore 612, 618
        }
    }
}
