//
// FSpotContext.cs
//
// Author:
//   Stephen Shaw <sshaw@decriptor.com>
//
// Copyright (C) 2019 Stephen Shaw
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using FSpot.Settings;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace FSpot.Database
{
    public partial class FSpotContext : DbContext
    {
        public FSpotContext()
        {
        }

        public FSpotContext(DbContextOptions<FSpotContext> options) : base(options)
        {
        }

        public virtual DbSet<Exports> Exports { get; set; }
        public virtual DbSet<JobsTable> Jobs { get; set; }
        public virtual DbSet<Meta> Meta { get; set; }
        public virtual DbSet<Photos> Photos { get; set; }
        public virtual DbSet<Rolls> Rolls { get; set; }
        public virtual DbSet<Tags> Tags { get; set; }

        // Unable to generate entity type for table 'photo_tags'. Please see the warning messages.
        // Unable to generate entity type for table 'photo_versions'. Please see the warning messages.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite($"Data Source={Path.Combine (Global.BaseDirectory, "photos.db")};");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Exports>(entity =>
            {
                entity.ToTable("exports");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.ExportToken)
                    .IsRequired()
                    .HasColumnName("export_token");

                entity.Property(e => e.ExportType)
                    .IsRequired()
                    .HasColumnName("export_type");

                entity.Property(e => e.ImageId).HasColumnName("image_id");

                entity.Property(e => e.ImageVersionId).HasColumnName("image_version_id");
            });

            modelBuilder.Entity<JobsTable>(entity =>
            {
                entity.ToTable("jobs");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.JobOptions)
                    .IsRequired()
                    .HasColumnName("job_options");

                entity.Property(e => e.JobPriority).HasColumnName("job_priority");

                entity.Property(e => e.JobType)
                    .IsRequired()
                    .HasColumnName("job_type");

                entity.Property(e => e.RunAt).HasColumnName("run_at");
            });

            modelBuilder.Entity<Meta>(entity =>
            {
                entity.ToTable("meta");

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Data).HasColumnName("data");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Photos>(entity =>
            {
                entity.ToTable("photos");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.BaseUri)
                    .IsRequired()
                    .HasColumnName("base_uri")
                    .HasColumnType("STRING");

                entity.Property(e => e.DefaultVersionId).HasColumnName("default_version_id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description");

                entity.Property(e => e.Filename)
                    .IsRequired()
                    .HasColumnName("filename")
                    .HasColumnType("STRING");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.RollId).HasColumnName("roll_id");

                entity.Property(e => e.Time).HasColumnName("time");
            });

            modelBuilder.Entity<Rolls>(entity =>
            {
                entity.ToTable("rolls");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Time).HasColumnName("time");
            });

            modelBuilder.Entity<Tags>(entity =>
            {
                entity.ToTable("tags");

                entity.HasIndex(e => e.Name)
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnName("category_id");

                entity.Property(e => e.Icon).HasColumnName("icon");

                entity.Property(e => e.IsCategory)
                    .HasColumnName("is_category")
                    .HasColumnType("BOOLEAN");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.SortPriority).HasColumnName("sort_priority");
            });
        }
    }
}
