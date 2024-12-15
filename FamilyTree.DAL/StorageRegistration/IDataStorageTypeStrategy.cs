using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FamilyTree.DAL.StorageRegistration
{
    public interface IDataStorageTypeStrategy
    {
        void RegisterRepositories(IServiceCollection services, IConfiguration configuration);
    }
}
