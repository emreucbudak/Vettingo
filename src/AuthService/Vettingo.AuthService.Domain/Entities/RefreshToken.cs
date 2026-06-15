namespace Vettingo.AuthService.Domain.Entities
{
    public class RefreshToken
    {
        public RefreshToken(string? token) 
        {
            this.Token = token;
            this.CreatedAt = DateTime.UtcNow;
            this.ExpiryTime = DateTime.UtcNow.AddDays(1);
        }

        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiryTime  { get; set; }
        public DateTime RevokeTime  { get; set; }
        public void UpdateToken(string token)
        {
            this.Token = token;
            this.CreatedAt = DateTime.UtcNow;
            this.ExpiryTime = DateTime.UtcNow.AddDays(1);
        }
        public void RevokeToken()
        {
            this.Token = null;
            this.RevokeTime = DateTime.UtcNow;
        }
        public RefreshToken FindRefreshToken(string token)
        {
            return this.Token == token ? this : null;
        }


    }
}
