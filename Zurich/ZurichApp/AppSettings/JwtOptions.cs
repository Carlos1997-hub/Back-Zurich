namespace ZurichApp.Api.AppSettings
{
    public class JwtOptions
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty; // mínimo 32 chars
        public int ExpirationMinutes { get; set; } = 60;
    }
}
