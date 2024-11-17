using MongoDB.Driver;

namespace SimpleWebAppReact.Services;

/// <summary>
/// MongoDB Service
/// </summary>
public class MongoDbService
{
    private readonly IConfiguration _configuration;
    private readonly IMongoDatabase? _database;
    
    /// <summary>
    /// constructor connects to database
    /// </summary>
    /// <param name="configuration"></param>
    public MongoDbService(IConfiguration configuration)
    {
        _configuration = configuration;
        
        var connectionString = _configuration.GetConnectionString("DbConnection");
        var databaseName = _configuration.GetConnectionString("DatabaseName");

        if (connectionString == null)
        {
            Console.WriteLine("\x1b[38;2;255;000;000m");
            Console.WriteLine("Error: connection URL secret is missing.");
            Console.WriteLine("Run the command in the Discord starting with");
            Console.WriteLine("'dotnet user-secrets', then try again.");
            
            Environment.Exit(1);
        }
        
        Console.WriteLine("connection information:");
        Console.WriteLine(connectionString);
        Console.WriteLine(databaseName);
        
        var mongoUrl = MongoUrl.Create(connectionString);
        var mongoClient = new MongoClient(mongoUrl);
        _database = mongoClient.GetDatabase(databaseName);
    }

    public IMongoDatabase? Database => _database;
}