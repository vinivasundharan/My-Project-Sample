using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TEDALS_Ver01.Models;
using System.Data.Entity.ModelConfiguration.Conventions;
using TrackerEnabledDbContext;

namespace TEDALS_Ver01.DAL
{
    public class TedalsContext : DbContext
    {
        public TedalsContext(): base("TedalsContext")
        {
            Database.SetInitializer<TedalsContext>(null);
        }

        public DbSet<Lsystem> Lsystem { get; set; }
        public DbSet<DataFormat> DataFormat { get; set; }
        public DbSet<LsystemFamily> LsystemFamily { get; set; }
        public DbSet<Option> Option { get; set; }
        public DbSet<OptionValue> OptionValue { get; set; }
        public DbSet<SetValue> SetValue { get; set; }
        public DbSet<TcSet> TcSet { get; set; }
        public DbSet<TechnicalCharacteristic> TechnicalCharacteristic { get; set; }
        public DbSet<UserRight> UserRight { get; set; }
        public DbSet<RevisionHistory> RevisionHistory { get; set; }
        public DbSet<Calculation> Calculation { get; set; }
        public DbSet<ConfigurationCollection> ConfigurationCollection { get; set; }
        public DbSet<Views> Views { get; set; }
        public DbSet<ViewsProperty> ViewsProperty { get; set; }
        public DbSet<ViewsCalculation> ViewsCalculation { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<FromSAP>();
            
        }

        public System.Data.Entity.DbSet<TEDALS_Ver01.Models.Config_OptionVal> Config_OptionVal { get; set; }
        public DbSet<FromSAP> FromSAP { get; set; }
        //public DbSet<FromSAP> FromSAP { get; set; }
      
    }

}