using BrugerServiceApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BrugerServiceApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;
    public UsersService(IOptions<UsersDbSettings> usersDbSettings)
    {
        var mongoClient = new MongoClient(usersDbSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(usersDbSettings.Value.DatabaseName);

        _usersCollection = mongoDatabase.GetCollection<User>(usersDbSettings.Value.CollectionName);
    }

    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.userID == id).FirstOrDefaultAsync();
}