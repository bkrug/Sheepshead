using Sheepshead.Model.Models;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;

namespace Sheepshead.Model.DAL
{
    public class GameContext : DbContext
    {
        public GameContext() : base(GetDbConnection(), true)
        {
        }

        private static DbConnection GetDbConnection()
        {
            
            var conn = new SqlConnection(
                        "metadata=res://*/Models.SheepsheadModel.csdl|res://*/Models.SheepsheadModel.ssdl|res://*/Models.SheepsheadModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source=DESKTOP-SVNAB17\\SQLEXPRESS;initial catalog=Sheepshead;integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;"
                        );
            return conn;
        }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
