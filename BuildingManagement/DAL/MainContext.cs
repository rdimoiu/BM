using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using BuildingManagement.Models;

namespace BuildingManagement.DAL
{
    public class MainContext : DbContext
    {
        public MainContext() : base("MainContext")
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<SubClient> SubClients { get; set; }

        public DbSet<Service> Services { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceType> InvoiceTypes { get; set; }

        public DbSet<DistributionMode> DistributionModes { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }
        public DbSet<MeterType> MeterTypes { get; set; }
        public DbSet<SubMeter> SubMeters { get; set; }
        
        public DbSet<Section> Sections { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<Space> Spaces { get; set; }
        public DbSet<SpaceType> SpaceTypes { get; set; }

        public DbSet<Cost> Costs { get; set; }

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

            //many to many Service-Section
            modelBuilder.Entity<Service>()
                .HasMany(m => m.Sections).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("SectionID")
                    .ToTable("ServiceSection"));

            //many to many Service-Level
            modelBuilder.Entity<Service>()
                .HasMany(m => m.Levels).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("LevelID")
                    .ToTable("ServiceLevel"));

            //many to many Service-Space
            modelBuilder.Entity<Service>()
                .HasMany(m => m.Spaces).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("SpaceID")
                    .ToTable("ServiceSpace"));

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