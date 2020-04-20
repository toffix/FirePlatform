﻿// <auto-generated />
using System;
using FirePlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FirePlatform.Models.Migrations
{
    [DbContext(typeof(Context))]
    [Migration("20200310164247_MainTemplates")]
    partial class MainTemplates
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.14-servicing-32113")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FirePlatform.Models.Models.MainTemplates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data");

                    b.Property<string>("Lng");

                    b.Property<string>("LongName");

                    b.Property<string>("ShortName");

                    b.HasKey("Id");

                    b.ToTable("MainTemplates");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.TemplateModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserForm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("FormId");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.HasIndex("UserId");

                    b.ToTable("UserForm");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.Users", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserTemplates", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte[]>("Data");

                    b.Property<string>("MainName");

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserTemplates");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserForm", b =>
                {
                    b.HasOne("FirePlatform.Models.Models.TemplateModel", "Form")
                        .WithMany("UserForms")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FirePlatform.Models.Models.Users", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserTemplates", b =>
                {
                    b.HasOne("FirePlatform.Models.Models.Users", "User")
                        .WithMany("UserTemplates")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
