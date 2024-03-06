using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DocumentService.Web.Authentications;

public class SigningSymmetricKey : IJwtSigningEncodingKey, IJwtSigningDecodingKey
{
    private readonly SymmetricSecurityKey _secretKey;

    public SigningSymmetricKey(string key)
    {
        _secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public string SigningAlgorithm { get; } = SecurityAlgorithms.HmacSha256;

    public SecurityKey GetKey()
    {
        return _secretKey;
    }
}