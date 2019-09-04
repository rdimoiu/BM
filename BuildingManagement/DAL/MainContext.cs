using BuildingManagement.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Policy;

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
        public virtual DbSet<SubMeter> SubMeters { get; set; }
        public virtual DbSet<SubSubMeter> SubSubMeters { get; set; }
        public virtual DbSet<MeterType> MeterTypes { get; set; }

        public virtual DbSet<MeterReading> MeterReadings { get; set; }
        public virtual DbSet<SubMeterReading> SubMeterReadings { get; set; }
        public virtual DbSet<SubSubMeterReading> SubSubMeterReadings { get; set; }

        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Level> Levels { get; set; }
        public virtual DbSet<Space> Spaces { get; set; }
        public virtual DbSet<SpaceType> SpaceTypes { get; set; }

        public virtual DbSet<UncountedCost> UncountedCosts { get; set; }
        public virtual DbSet<CountedCost> CountedCosts { get; set; }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<AbstractMeter>().ToTable("Meter");
            modelBuilder.Entity<AbstractMeterReading>().ToTable("MeterReading");

            //many to many AbstractMeter-MeterType
            modelBuilder.Entity<AbstractMeter>()
                .HasMany(am => am.MeterTypes).WithMany(mt => mt.AbstractMeters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("MeterTypeID")
                    .ToTable("MeterMeterType"));

            //many to many AbstractMeter-Space
            modelBuilder.Entity<AbstractMeter>()
                .HasMany(am => am.Spaces).WithMany(s => s.AbstractMeters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("SpaceID")
                    .ToTable("MeterSpace"));

            //many to many AbstractMeter-Level
            modelBuilder.Entity<AbstractMeter>()
                .HasMany(am => am.Levels).WithMany(l => l.AbstractMeters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("LevelID")
                    .ToTable("MeterLevel"));

            //many to many AbstractMeter-Section
            modelBuilder.Entity<AbstractMeter>()
                .HasMany(am => am.Sections).WithMany(s => s.AbstractMeters)
                .Map(t => t.MapLeftKey("MeterID")
                    .MapRightKey("SectionID")
                    .ToTable("MeterSection"));

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

            //many to many Service-Space
            modelBuilder.Entity<Service>()
                .HasMany(s => s.Spaces).WithMany(s => s.Services)
                .Map(t => t.MapLeftKey("ServiceID")
                    .MapRightKey("SpaceID")
                    .ToTable("ServiceSpace"));

            //composed primary key for UncountedCost 
            modelBuilder.Entity<UncountedCost>().HasKey(uc => new { uc.ServiceID, uc.SpaceID });
            modelBuilder.Entity<CountedCost>().HasKey(cc => new { cc.ServiceID, cc.SpaceID, cc.AbstractMeterID });

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
            //modelBuilder.Entity<AbstractMeter>().Ignore(m => m.Spaces);
        }
    }
}