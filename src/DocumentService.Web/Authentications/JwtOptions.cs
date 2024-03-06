namespace DocumentService.Web.Authentications;

public class JwtOptions
{
    public const string JWTBearer = "JWTBearer";

    public string Issuer { get; set; } = string.Empty;
    public int ExpiresInMinutes { get; set; } = 5;
}
