using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SmartHub.DataContext.DbModels;

namespace SmartHub.DataContext
{
    public class DataDbContext : DbContext
    {
        public DbSet<GroupEntity> GroupEntities { get; set; }
        public DbSet<RelationshipGroupAndRole> RelationshipGroupsAndroles { get; set; }
        public DbSet<RelationshipUserAndRole> RelationshipUserAndRoles { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "dataContext.db" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            optionsBuilder.UseSqlite(connection);
        }
    }
}
