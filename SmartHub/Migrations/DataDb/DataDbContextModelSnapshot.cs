﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartHub.DataContext;

#nullable disable

namespace SmartHub.Migrations.DataDb
{
    [DbContext(typeof(DataDbContext))]
    partial class DataDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0");

            modelBuilder.Entity("SmartHub.DataContext.DbModels.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.DeviceInterfaceItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Control")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("DataType")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("Interfaces");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.GroupDevice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("DeviceId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GroupEntityId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("GroupEntityId");

                    b.ToTable("GroupDevices");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.GroupEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("DeviceId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.ToTable("GroupEntities");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.RelationshipGroupAndRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("GroupEntityId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("IdRole")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NameGroup")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("idGroup")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("GroupEntityId");

                    b.ToTable("RelationshipGroupsAndroles");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.RelationshipUserAndRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("IdRole")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("idUser")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("RelationshipUserAndRoles");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.DeviceInterfaceItem", b =>
                {
                    b.HasOne("SmartHub.DataContext.DbModels.Device", null)
                        .WithMany("Interfaces")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.GroupDevice", b =>
                {
                    b.HasOne("SmartHub.DataContext.DbModels.Device", "Device")
                        .WithMany("GroupDevices")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SmartHub.DataContext.DbModels.GroupEntity", "GroupEntity")
                        .WithMany("GroupDevices")
                        .HasForeignKey("GroupEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Device");

                    b.Navigation("GroupEntity");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.GroupEntity", b =>
                {
                    b.HasOne("SmartHub.DataContext.DbModels.Device", null)
                        .WithMany("Groups")
                        .HasForeignKey("DeviceId");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.RelationshipGroupAndRole", b =>
                {
                    b.HasOne("SmartHub.DataContext.DbModels.GroupEntity", null)
                        .WithMany("Roles")
                        .HasForeignKey("GroupEntityId");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.Device", b =>
                {
                    b.Navigation("GroupDevices");

                    b.Navigation("Groups");

                    b.Navigation("Interfaces");
                });

            modelBuilder.Entity("SmartHub.DataContext.DbModels.GroupEntity", b =>
                {
                    b.Navigation("GroupDevices");

                    b.Navigation("Roles");
                });
#pragma warning restore 612, 618
        }
    }
}
