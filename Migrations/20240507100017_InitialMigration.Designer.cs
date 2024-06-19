﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using YnovPassword.modele;

#nullable disable

namespace YnovPassword.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20240507100017_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.4");

            modelBuilder.Entity("YnovPassword.modele.Configuration", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("VersionMajeure")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("VersionMineure")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Configurations");
                });

            modelBuilder.Entity("YnovPassword.modele.Dictionnaire", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Mot")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Dictionnaires");
                });

            modelBuilder.Entity("YnovPassword.modele.Dossiers", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Dossiers");
                });

            modelBuilder.Entity("YnovPassword.modele.ProfilsData", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DossiersID")
                        .HasColumnType("TEXT");

                    b.Property<string>("EncryptedPassword")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UtilisateursID")
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.HasIndex("DossiersID");

                    b.HasIndex("UtilisateursID");

                    b.ToTable("ProfilsData");
                });

            modelBuilder.Entity("YnovPassword.modele.Utilisateurs", b =>
                {
                    b.Property<Guid>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.HasKey("ID");

                    b.ToTable("Utilisateurs");
                });

            modelBuilder.Entity("YnovPassword.modele.ProfilsData", b =>
                {
                    b.HasOne("YnovPassword.modele.Dossiers", "Dossiers")
                        .WithMany("ProfilsData")
                        .HasForeignKey("DossiersID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("YnovPassword.modele.Utilisateurs", "Utilisateurs")
                        .WithMany("ProfilsData")
                        .HasForeignKey("UtilisateursID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Dossiers");

                    b.Navigation("Utilisateurs");
                });

            modelBuilder.Entity("YnovPassword.modele.Dossiers", b =>
                {
                    b.Navigation("ProfilsData");
                });

            modelBuilder.Entity("YnovPassword.modele.Utilisateurs", b =>
                {
                    b.Navigation("ProfilsData");
                });
#pragma warning restore 612, 618
        }
    }
}
