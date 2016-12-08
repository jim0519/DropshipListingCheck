using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Common;
using Common.Models;
using DropshipListingCheckData.Models.Mapping;
//using DropshipListingCheckData.Models.Mapping;

namespace DropshipListingCheckData
{
    public partial class AppDBContext : DbContext, IDbContext
    {
        static AppDBContext()
        {
            Database.SetInitializer<AppDBContext>(null);
        }

        public AppDBContext()
            : base(Config.Instance.ConnectionString)
        {
        }

        //public DbSet<AutoPostAdHeader> AutoPostAdHeaders { get; set; }
        //public DbSet<AutoPostAdLine> AutoPostAdLines { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new DealsdirectDataMap());
            modelBuilder.Configurations.Add(new OnlyonlineDataMap());

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, int? timeout = null, params object[] parameters)
        {
            int? previousTimeout = null;
            if (timeout.HasValue)
            {
                //store previous timeout
                previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
            }

            var result = this.Database.ExecuteSqlCommand(sql, parameters);

            if (timeout.HasValue)
            {
                //Set previous timeout back
                ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
            }

            //return result
            return result;
        }
    }
}
