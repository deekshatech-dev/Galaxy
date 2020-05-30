using MySql.Data.Entity;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GPSDB
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("name=MySQLConnection")
        {

        }

        public DbSet<MapPoints> MapPoints { get; set; }

        public DbSet<MapAttributes> MapAttributes { get; set; }

        public DbSet<nokiaxmldetail> nokiaxmldetail { get; set; }

        public DbSet<ericssonxmldetail> ericssonxmldetail { get; set; }

        public DbSet<ericsson_5gmaster> ericsson_5gmaster { get; set; }

        public DbSet<ericsson_5gdetail> ericsson_5gdetail { get; set; }

        public DbSet<Ericsson_KPI_5g_Data> ericsson_kpi_5g_data { get; set; }
        public DbSet<Ericsson_KPI_Data> ericsson_kpi_data { get; set; }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
