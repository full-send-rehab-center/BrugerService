using BrugerServiceApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography;
using System;
using System.Text;

namespace BrugerServiceApi.Services;

public class UsersService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly string _collectionName;
    private readonly HashingService _hashingService;
    public UsersService(IOptions<UsersDbSettings> usersDbSettings, IConfiguration config, HashingService hashingService)
    {
        _collectionName = config["CollectionName"];
        _connectionString = config["ConnectionString"];
        _databaseName = config["DatabaseName"];

        _hashingService = hashingService;

        var mongoClient = new MongoClient(_connectionString);
        var mongoDatabase = mongoClient.GetDatabase(_databaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(_collectionName);
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
    public async Task CreateAsync(User newUser)
    {
        string _salt = _hashingService.CreateSalt();
        string _hashedPassword = _hashingService.HashPassword(newUser.password, _salt);
        newUser.password = _hashedPassword;
        newUser.salt = _salt;
        await _usersCollection.InsertOneAsync(newUser);
    }

    // Update methods
    // Update User
    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.userID == id, updatedUser);

    // Delete methods
    // Delete User by ID
    public async Task DeleteAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.userID == id);
}