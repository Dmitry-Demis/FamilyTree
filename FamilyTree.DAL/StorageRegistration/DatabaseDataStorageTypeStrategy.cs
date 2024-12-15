using FamilyTree.DAL.Model;
using FamilyTree.DAL.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.DAL.StorageRegistration
{
    public class DatabaseDataStorageTypeStrategy : IDataStorageTypeStrategy
    {
        public void RegisterRepositories(IServiceCollection services, IConfiguration configuration)
        {
            var basePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\.."));
            var dbSettings = configuration.GetSection("DatabaseSettings");
            var databasePath = dbSettings["DatabasePath"]
                               ?? throw new InvalidOperationException("DatabasePath is missing in the configuration.");
            var connectionString = dbSettings["ConnectionString"]
                                   ?? throw new InvalidOperationException("ConnectionString is missing in the configuration.");
            
            var fullDbPath = Path.Combine(basePath, databasePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullDbPath)!);

            services
                .AddDbContext<FamilyTreeDbContext>(options =>
                options.UseSqlite(connectionString.Replace(databasePath, fullDbPath)))
                .AddScoped<IRepository<Person>, DbRepository<Person>>()
                ;
        }
    }

}
