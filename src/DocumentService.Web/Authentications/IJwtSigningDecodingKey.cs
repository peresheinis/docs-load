using Microsoft.IdentityModel.Tokens;

namespace DocumentService.Web.Authentications;

// Ключ для проверки подписи (публичный)
public interface IJwtSigningDecodingKey
{
    SecurityKey GetKey();
}
