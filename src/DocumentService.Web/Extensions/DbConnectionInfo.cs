using Npgsql;

namespace DocumentService.Web.Extensions;

public class DbConnectionInfo
{
    public const string ConfName = "DbPgConn";
    public const int DefaultPort = 5432;
    public string User { get; set; }
    public string Password { get; set; }
    public string Database { get; set; }
    /// <summary>
    /// IP:Port
    /// </summary>
    public string Host { get; set; }

    public string GetNpgsqlConnectionString()
    {
        var splitHost = Host.Split(':');
        var host = splitHost[0];
        var port = splitHost.Length == 2 ? int.Parse(splitHost[1]) : DefaultPort;
        var stringBuilder = new NpgsqlConnectionStringBuilder();

        stringBuilder.Username = User;
        stringBuilder.Password = Password;
        stringBuilder.Host = host;
        stringBuilder.Port = port;
        stringBuilder.Database = Database;

        return stringBuilder.ConnectionString;
    }
}
