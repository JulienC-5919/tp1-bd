public class ConnectionStringBuilder
{
    public ConnectionStringBuilder(params string[] args) {
        if (args.Length > 0) host = args[0];
        if (args.Length > 1) username = args[1];
        if (args.Length > 2) database = args[2];
    }
    public String? host = null;
    public String? username = null;
    public String? password = null;
    public String? database = null;

    public string GetConnectionString()
    {
        if (database == null || password == null || host == null || username == null)
            return "";
        return $"Host={host};Username={username};Password={password};Database={database}";
    }
}