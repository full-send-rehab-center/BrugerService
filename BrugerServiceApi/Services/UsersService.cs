using BrugerServiceApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace BrugerServiceApi.Services;

public interface IUsersService
{
    Task<User> CreateAsync(User newUser);
    Task DeleteAsync(string id);
    Task<List<User>> GetAsync();
    Task<User?> GetAsync(string id);
    Task UpdateAsync(string id, User updatedUser);
}


public class UsersService : IUsersService
{
    private readonly IMongoCollection<User> _usersCollection;
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly string _collectionName;
    private IConfiguration _config;

    public UsersService(IOptions<UsersDbSettings> usersDbSettings, IConfiguration config)
    {

        _config = config;

        _collectionName = config["CollectionName"];
        _connectionString = config["ConnectionString"];
        _databaseName = config["DatabaseName"];

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
    public async Task<User?> CreateAsync(User newUser)
    {
        string _salt = CreateSalt();
        string _hashedPassword = HashPassword(newUser.password, _salt);
        newUser.password = _hashedPassword;
        newUser.salt = _salt;
        await _usersCollection.InsertOneAsync(newUser);
        return newUser;
    }

    // Update methods
    // Update User
    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.userID == id, updatedUser);

    // Delete methods
    // Delete User by ID
    public async Task DeleteAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.userID == id);


    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Hashing and Salt methods
    public string CreateSalt ()   
    {
        byte[] saltBytes = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetNonZeroBytes(saltBytes);
        }
        string salt = Convert.ToBase64String(saltBytes);
        return salt;
    }

    public string HashPassword(string password, string salt)
    {
        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: Convert.FromBase64String(salt),
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return hashed;
    }
}