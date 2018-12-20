﻿// <auto-generated />
using FirePlatform.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FirePlatform.Models.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.0-rtm-35687")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FirePlatform.Models.Models.TemplateModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserForm", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("FormId");

                    b.Property<int>("Id");

                    b.HasKey("UserId", "FormId");

                    b.HasAlternateKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("UserForm");
                });

            modelBuilder.Entity("FirePlatform.Models.Models.UserForm", b =>
                {
                    b.HasOne("FirePlatform.Models.Models.TemplateModel", "TemplateModel")
                        .WithMany("UserForms")
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("FirePlatform.Models.Models.User", "User")
                        .WithMany("UserForms")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
