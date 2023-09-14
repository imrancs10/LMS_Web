﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LearningManagementSystem.Models;

public partial class LMSContext : DbContext
{
    public LMSContext(DbContextOptions<LMSContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Student> Student { get; set; }
    public virtual DbSet<StudentFile> StudentFile { get; set; }
    public virtual DbSet<Lookup> Lookup { get; set; }
    public virtual DbSet<UserDetail> UserDetail { get; set; }
    public virtual DbSet<StudentCredential> StudentCredential { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.AadhaarNumber)
                .IsRequired()
                .HasMaxLength(12);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.MobileNumber)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.ModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.RollNumber)
                .IsRequired()
                .HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}