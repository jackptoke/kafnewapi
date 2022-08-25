using MinimalApi.Models;
using System.Data.Entity;
using DbContext = System.Data.Entity.DbContext;

namespace MinimalApi.Persistence
{
    public class ArticleContext : DbContext
    {
        public ArticleContext(string connectionString)
        {
            Database.SetInitializer<ArticleContext>(new DropCreateDatabaseAlways<ArticleContext>());
            this.Database.Connection.ConnectionString = connectionString;
        }
        public virtual DbSet<Article> Articles { get; set; }
    }
}
