using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace helpers
{
    public class jwt
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInMinutes;

        public jwt(string secret_key, string issuer, string audience, int expires_in_minutes)
        {
            _secretKey = secret_key;
            _issuer = issuer;
            _audience = audience;
            _expiresInMinutes = expires_in_minutes;
        }

        public string createToken(List<Claim> auth_claims)
        {
            var _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var _token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                expires: DateTime.Now.ToLocalTime().AddMinutes(_expiresInMinutes),
                claims: auth_claims,
                signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(_token);
        }

        public JwtSecurityToken validateToken(string token)
        {
            var _tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            try
            {
                _tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                }, out SecurityToken validatedToken);

                JwtSecurityToken _jwtToken = (JwtSecurityToken)validatedToken;
                return _jwtToken;
            }
            catch
            {
                // Token validation failed
                return null;
            }
        }
    }
}