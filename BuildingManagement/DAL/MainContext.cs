using BuildingManagement.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace BuildingManagement.DAL
{
    public class MainContext : DbContext
    {
        public MainContext() : base("MainContext")
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<SubClient> SubClients { get; set; }

        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<Provider> Providers { get; set; }
        public virtual DbSet<Invoice> Invoices { get; set; }
        public virtual DbSet<InvoiceType> InvoiceTypes { get; set; }

        public virtual DbSet<Meter> Meters { get; set; }
        public virtual DbSet<MeterReading> MeterReadings { get; set; }
        public virtual DbSet<MeterType> MeterTypes { get; set; }
        public virtual DbSet<SubMeter> SubMeters { get; set; }
        public virtual DbSet<SubMeterReading> SubMeterReadings { get; set; }
        public virtual DbSet<SubSubMeter> SubSubMeters { get; set; }
        public virtual DbSet<SubSubMeterReading> SubSubMeterReadings { get; set; }

        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Space> Spaces { get; set; }
        public virtual DbSet<SpaceType> SpaceTypes { get; set; }

        public virtual DbSet<Cost> Costs { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //many to many Meter-MeterType
            modelBuilder.Entity<Meter>()
                .HasMany(m => m.MeterTypes).WithMany(mt => mt.Meters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("MeterTypeID")
                    .ToTable("MeterMeterType"));

            //many to many Meter-Space
            modelBuilder.Entity<Meter>()
                .HasMany(m => m.Spaces).WithMany(s => s.Meters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("SpaceID")
                    .ToTable("MeterSpace"));

            //many to many Meter-Level
            modelBuilder.Entity<Meter>()
                .HasMany(m => m.Levels).WithMany(s => s.Meters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("LevelID")
                    .ToTable("MeterLevel"));

            //many to many Meter-Section
            modelBuilder.Entity<Meter>()
                .HasMany(m => m.Sections).WithMany(s => s.Meters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("SectionID")
                    .ToTable("MeterSection"));

            //many to many SubMeter-MeterType
            modelBuilder.Entity<SubMeter>()
                .HasMany(sm => sm.MeterTypes).WithMany(mt => mt.SubMeters)
                .Map(t => t.MapLeftKey("SubMeterID")
                    .MapRightKey("MeterTypeID")
                    .ToTable("SubMeterMeterType"));

            //many to many SubMeter-Space
            modelBuilder.Entity<SubMeter>()
                .HasMany(sm => sm.Spaces).WithMany(s => s.SubMeters)
                .Map(t => t.MapLeftKey("SubMeterID")
                    .MapRightKey("SpaceID")
                    .ToTable("SubMeterSpace"));

            //many to many SubMeter-Level
            modelBuilder.Entity<SubMeter>()
                .HasMany(sm => sm.Levels).WithMany(s => s.SubMeters)
                .Map(t => t.MapLeftKey("SubMeterID")
                    .MapRightKey("LevelID")
                    .ToTable("SubMeterLevel"));

            //many to many SubMeter-Section
            modelBuilder.Entity<SubMeter>()
                .HasMany(sm => sm.Sections).WithMany(s => s.SubMeters)
                .Map(t => t.MapLeftKey("SubMeterID")
                    .MapRightKey("SectionID")
                    .ToTable("SubMeterSection"));

            //many to many SubSubMeter-MeterType
            modelBuilder.Entity<SubSubMeter>()
                .HasMany(ssm => ssm.MeterTypes).WithMany(mt => mt.SubSubMeters)
                .Map(t => t.MapLeftKey("SubSubMeterID")
                    .MapRightKey("MeterTypeID")
                    .ToTable("SubSubMeterMeterType"));

            //many to many SubSubMeter-Space
            modelBuilder.Entity<SubSubMeter>()
                .HasMany(ssm => ssm.Spaces).WithMany(s => s.SubSubMeters)
                .Map(t => t.MapLeftKey("SubSubMeterID")
                    .MapRightKey("SpaceID")
                    .ToTable("SubSubMeterSpace"));

            //many to many SubSubMeter-Level
            modelBuilder.Entity<SubSubMeter>()
                .HasMany(ssm => ssm.Levels).WithMany(s => s.SubSubMeters)
                .Map(t => t.MapLeftKey("SubSubMeterID")
                    .MapRightKey("LevelID")
                    .ToTable("SubSubMeterLevel"));

            //many to many SubSubMeter-Section
            modelBuilder.Entity<SubSubMeter>()
                .HasMany(ssm => ssm.Sections).WithMany(s => s.SubSubMeters)
                .Map(t => t.MapLeftKey("SubSubMeterID")
                    .MapRightKey("SectionID")
                    .ToTable("SubSubMeterSection"));

            //many to many Service-Section
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Sections).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("SectionID")
                    .ToTable("ServiceSection"));

            //many to many Service-Level
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Levels).WithMany(l => l.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("LevelID")
                    .ToTable("ServiceLevel"));

            //many to many Service-Space
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Spaces).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("SpaceID")
                    .ToTable("ServiceSpace"));

            //composed primary key for Cost 
            modelBuilder.Entity<Cost>().HasKey(c => new { c.ServiceID, c.SpaceID });

            //unique constraint Number for Space
            //modelBuilder.Entity<Space>()
            //    .Property(s => s.Number)
            //    .IsRequired()
            //    .HasMaxLength(50)
            //    .HasColumnAnnotation(
            //        "Index",
            //        new IndexAnnotation(new[]
            //            {
            //                new IndexAttribute("Index") { IsUnique = true }
            //            }));

            //dont map Spaces
            //modelBuilder.Entity<Meter>().Ignore(m => m.Spaces);
        }
    }
}