using MongoDB.Driver;
using System.Threading.Tasks;

namespace Authservice.API
{
    public class UserRepository
    {
        private readonly IMongoCollection<UserModel> _users;

        public UserRepository(IConfiguration config)
        {
            var connectionString = config.GetValue<string>("ConnectionString");
            var mongoUrl = new MongoUrl(connectionString);
            var client = new MongoClient(mongoUrl);
            var database = client.GetDatabase(mongoUrl.DatabaseName);
            _users = database.GetCollection<UserModel>("users");
        }

        public async Task CreateUserAsync(UserModel user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task<UserModel> GetByUserNameAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetByVerificationTokenAsync(string token)
        {
            return await _users.Find(u => u.VerificationToken == token).FirstOrDefaultAsync();
        }

        public async Task UpdateUserAsync(UserModel user)
        {
            await _users.ReplaceOneAsync(u => u.Id == user.Id, user);
        }

        public async Task DeleteExpiredUnverifiedUsersAsync()
        {
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(u => u.EmailVerified, false),
                Builders<UserModel>.Filter.Lt(u => u.VerificationTokenExpiry, DateTime.UtcNow)
            );
            await _users.DeleteManyAsync(filter);
        }

        public async Task DeleteUser(string email)
        {
            var filter = Builders<UserModel>.Filter.Eq(u => u.Email, email);
            await _users.DeleteOneAsync(filter);
        }
    }
}