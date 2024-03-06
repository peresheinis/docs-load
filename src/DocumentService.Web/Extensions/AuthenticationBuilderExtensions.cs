using DocumentService.Web.Authentications;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace DocumentService.Web.Extensions;

public static class AuthenticationBuilderExtensions
{
    public static void SetupAuthentication(this IServiceCollection services, IConfiguration configuration, Action<JwtBearerOptions>? options = null)
    {
        JwtOptions? jwtOptions = configuration
            .GetSection(JwtOptions.JWTBearer)
            .Get<JwtOptions>();

        if (jwtOptions is null)
        {
            throw new ArgumentNullException(JwtOptions.JWTBearer);
        }

        string? signingSecurityKey = configuration[$"{JwtOptions.JWTBearer}:SecurityKey"];

        if (signingSecurityKey is null)
        {
            throw new ArgumentNullException($"{JwtOptions.JWTBearer}:SecurityKey");
        }

        if (options is null)
        {
            SigningSymmetricKey signingKey = new(signingSecurityKey);
            IJwtSigningDecodingKey signingDecodingKey = signingKey;

            services
                .AddSingleton<IJwtSigningEncodingKey>(signingKey);

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options ?? GetDefaultOptions(jwtOptions, signingDecodingKey));
        }
        else
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options);
        }
    }

    private static Action<JwtBearerOptions> GetDefaultOptions(JwtOptions jwtOptions, IJwtSigningDecodingKey jwtSigningDecodingKey)
    {
        return options => options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = jwtOptions.Issuer ?? throw new ArgumentNullException($"{JwtOptions.JWTBearer}:Issuer"),
            ValidateAudience = false,
            ValidateActor = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtSigningDecodingKey.GetKey()
        };
    }
}
