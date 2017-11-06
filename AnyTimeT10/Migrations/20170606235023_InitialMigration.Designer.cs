using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using AnyTimeT10.DAL;

namespace AnyTimeT10.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20170606235023_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

            modelBuilder.Entity("AnyTimeT10.Models.DailyTask", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CategoryId");

                    b.Property<int>("Completed")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<DateTime?>("Date");

                    b.Property<DateTime?>("Deadline");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<int>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("UserId");

                    b.ToTable("DailyTasks");
                });

            modelBuilder.Entity("AnyTimeT10.Models.TaskCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ColorCode");

                    b.Property<string>("Icon");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("TaskCategories");
                });

            modelBuilder.Entity("AnyTimeT10.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AvatarFilename");

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AnyTimeT10.Models.DailyTask", b =>
                {
                    b.HasOne("AnyTimeT10.Models.TaskCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AnyTimeT10.Models.User", "User")
                        .WithMany("DailyTasks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
