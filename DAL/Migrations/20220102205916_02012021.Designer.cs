// <auto-generated />
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
    [Migration("20220102205916_02012021")]
    partial class _02012021
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.EF.Entities.ArtDirections", b =>
                {
                    b.Property<int>("ArtDirectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ArtDirectionId");

                    b.ToTable("ArtDirections");
                });

            modelBuilder.Entity("DAL.EF.Entities.Author", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("Country")
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
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("EndTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("StartTime")
                        .HasColumnType("datetime2");

                    b.HasKey("CollectionId");

                    b.ToTable("Collections");
                });

            modelBuilder.Entity("DAL.EF.Entities.Country", b =>
                {
                    b.Property<int>("CountryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CountryId");

                    b.ToTable("Countries");
                });

            modelBuilder.Entity("DAL.EF.Entities.Exhibit", b =>
                {
                    b.Property<int>("ExhibitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("AuthorId")
                        .HasColumnType("int");

                    b.Property<int?>("CollectionId")
                        .HasColumnType("int");

                    b.Property<long?>("Cost")
                        .HasColumnType("bigint");

                    b.Property<int?>("Country")
                        .HasColumnType("int");

                    b.Property<int?>("CreationYear")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Direction")
                        .HasColumnType("int");

                    b.Property<string>("Materials")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("ExhibitId");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CollectionId");

                    b.ToTable("Exhibits");
                });

            modelBuilder.Entity("DAL.EF.Entities.Exhibit_Type", b =>
                {
                    b.Property<int>("ExhibitTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ExhibitTypeId");

                    b.ToTable("ExhibitTypes");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopAuthor", b =>
                {
                    b.Property<int>("AuthorId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AuthorId1")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.HasKey("AuthorId");

                    b.HasIndex("AuthorId1");

                    b.ToTable("PopAuthors");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopCollection", b =>
                {
                    b.Property<int>("CollectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CollectionId1")
                        .HasColumnType("int");

                    b.Property<int>("Rate")
                        .HasColumnType("int");

                    b.HasKey("CollectionId");

                    b.HasIndex("CollectionId1");

                    b.ToTable("PopCollections");
                });

            modelBuilder.Entity("DAL.EF.Entities.PopExhibit", b =>
                {
                    b.Property<int>("ExhibitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .WithMany("Exhibits")
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

            modelBuilder.Entity("DAL.EF.Entities.PopCollection", b =>
                {
                    b.HasOne("DAL.EF.Entities.Collection", "Collection")
                        .WithMany()
                        .HasForeignKey("CollectionId1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
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
                    b.Navigation("Exhibits");
                });

            modelBuilder.Entity("DAL.EF.Entities.Collection", b =>
                {
                    b.Navigation("Exhibits");
                });
#pragma warning restore 612, 618
        }
    }
}
