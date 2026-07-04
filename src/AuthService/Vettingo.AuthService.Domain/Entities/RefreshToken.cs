namespace Vettingo.AuthService.Domain.Entities
{
    public class RefreshToken
    {
        public RefreshToken(string? token, Guid userId) 
        {
            CheckRefreshTokenContent(token, userId);
            this.Token = token;
            this.CreatedAt = DateTime.UtcNow;
            this.ExpiryTime = DateTime.UtcNow.AddDays(1);
            this.RevokeTime = null;
            this.UserId = userId;
        }

        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string? Token { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiryTime { get; private set; }
        public DateTime? RevokeTime { get; private set; }

        public void UpdateToken(string token)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(token, nameof(token));
            this.Token = token;
            this.CreatedAt = DateTime.UtcNow;
            this.ExpiryTime = DateTime.UtcNow.AddDays(1);
            this.RevokeTime = null;
        }

        public void RevokeToken()
        {
            this.Token = null;
            this.RevokeTime = DateTime.UtcNow;
        }

        public RefreshToken? FindRefreshToken(string token)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(token, nameof(token));
            return this.Token == token ? this : null;
        }

        public static void CheckRefreshTokenContent(string? token, Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("UserId boş olamaz.", nameof(userId));
            }

            ArgumentNullException.ThrowIfNullOrWhiteSpace(token, nameof(token));
        }
    }
}
