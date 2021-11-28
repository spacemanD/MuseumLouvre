﻿// <auto-generated />
using System;
using DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20211128183617_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0");

            modelBuilder.Entity("DAL.EF.Entities.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Country")
                        .HasColumnType("int");

                    b.Property<int>("CountryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("DeathDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("AuthorId");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("DAL.EF.Entities.Collection", b =>
                {
                    b.Property<int>("CollectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("CollectionId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("DAL.EF.Entities.Exhibit", b =>
                {
                    b.Property<int>("ExhibitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int?>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int?>("CollectionId")
                        .HasColumnType("int");

                    b.Property<long?>("Cost")
                        .HasColumnType("bigint");

                    b.Property<int?>("Country")
                        .HasColumnType("int");

                    b.Property<int?>("CountryId")
                        .HasColumnType("int");

                    b.Property<int?>("CreationYear")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Direction")
                        .HasColumnType("int");

                    b.Property<int?>("DirectionId")
                        .HasColumnType("int");

                    b.Property<string>("Materials")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("TypeId")
                        .HasColumnType("int");

                    b.HasKey("ExhibitId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CollectionId");

                    b.ToTable("Exhibits");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopAuthor", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("AuthorId1")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.HasKey("AuthorId");

                    b.HasIndex("AuthorId1");

                    b.ToTable("PopAuthors");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopExhibit", b =>
                {
                    b.Property<int>("ExhibitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .UseIdentityColumn();

                    b.Property<int>("ExhibitId1")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.HasKey("ExhibitId");

                    b.HasIndex("ExhibitId1");

                    b.ToTable("PopExhibits");
                });

            modelBuilder.Entity("DAL.EF.Entities.Exhibit", b =>
                {
                    b.HasOne("DAL.EF.Entities.Author", "Author")
                        .WithMany("Pictures")
                        .HasForeignKey("AuthorId");

                    b.HasOne("DAL.EF.Entities.Collection", "Collection")
                        .WithMany("Exhibits")
                        .HasForeignKey("CollectionId");

                    b.Navigation("Author");

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopAuthor", b =>
                {
                    b.HasOne("DAL.EF.Entities.Author", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopExhibit", b =>
                {
                    b.HasOne("DAL.EF.Entities.Exhibit", "Exhibit")
                        .WithMany()
                        .HasForeignKey("ExhibitId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Exhibit");
                });

            modelBuilder.Entity("DAL.EF.Entities.Author", b =>
                {
                    b.Navigation("Pictures");
                });

            modelBuilder.Entity("DAL.EF.Entities.Collection", b =>
                {
                    b.Navigation("Exhibits");
                });
#pragma warning restore 612, 618
        }
    }
}
