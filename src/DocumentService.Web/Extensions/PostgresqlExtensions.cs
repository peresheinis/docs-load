using Microsoft.EntityFrameworkCore;
using System.Data.Entity.Infrastructure;

namespace DocumentService.Web.Extensions;

public static class Extensions
{
    public static void SetupNpgsql<TContext>(this IServiceCollection services, ConfigurationManager configuration)
        where TContext : DbContext
    {
        var connectionInfo = configuration
                .GetSection(DbConnectionInfo.ConfName)
                .Get<DbConnectionInfo>();

        var connStr = connectionInfo.GetNpgsqlConnectionString();
        services.AddDbContext<TContext>(opt => opt.UseNpgsql(connStr));
    }
}
