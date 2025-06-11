using MongoDB.Bson.Serialization.Attributes;

namespace Authservice.API
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string? Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public bool EmailVerified { get; set; } = false;
        public string VerificationToken { get; set; }
        public DateTime? VerificationTokenExpiry { get; set; }

        public UserModel(string userName, string password, string email)
        {
            UserName = userName;
            Password = password;
            Email = email;
            VerificationToken = Guid.NewGuid().ToString();
            VerificationTokenExpiry = DateTime.UtcNow.AddDays(1);
        }
    }
}
