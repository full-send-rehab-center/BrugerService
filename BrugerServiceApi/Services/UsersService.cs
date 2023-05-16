using BrugerServiceApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BrugerServiceApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly string _connectionString;
    public UsersService(IOptions<UsersDbSettings> usersDbSettings, IConfiguration config)
    {
        _connectionString = config["mongodb://mongodb:27017"];
        
        var mongoClient = new MongoClient(usersDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDbSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(usersDbSettings.Value.CollectionName);
    }

    // Get methods
    // Get all Users
    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(s => true).ToListAsync();
    // Get User by ID
    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.userID == id).FirstOrDefaultAsync();

    // Post methodss
    // Post new User
    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    // Update methods
    // Update User
    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.userID == id, updatedUser);

    // Delete methods
    // Delete User by ID
    public async Task DeleteAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.userID == id);
}