using Microsoft.IdentityModel.Tokens;

namespace DocumentService.Web.Authentications;

// Ключ для формирования подписи (приватный)
public interface IJwtSigningEncodingKey
{
    string SigningAlgorithm { get; }

    SecurityKey GetKey();
}

