using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace TaskList.Services
{
    public static class TokenService
    {
        private static SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SXkSqsKyNUyvGbnHs7ke2NCq8zQzNLW7mPmHbnZZ"));
        private static string issuer = "https://fbi-demo.com";
        public static SecurityToken GetToken(List<Claim> claims) =>
            new JwtSecurityToken(
                issuer,
                issuer,
                claims,
                expires: DateTime.Now.AddDays(30.0),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

        public static TokenValidationParameters GetTokenValidationParameters() =>
            new TokenValidationParameters
            {
                ValidIssuer = issuer,
                ValidAudience = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("SXkSqsKyNUyvGbnHs7ke2NCq8zQzNLW7mPmHbnZZ")),
                ClockSkew = TimeSpan.Zero // remove delay of token when expire
            };

        public static string WriteToken(SecurityToken token) =>
            new JwtSecurityTokenHandler().WriteToken(token);

        public static SecurityToken TestJwtSecurityTokenHandler()
        {
            var stream =
                "eyJhbGciOiJSUzI1NiJ9.eyJpc3MiOiJJU1MiLCJzY29wZSI6Imh0dHBzOi8vbGFyaW0uZG5zY2UuZG91YW5lL2NpZWxzZXJ2aWNlL3dzIiwiYXVkIjoiaHR0cHM6Ly9kb3VhbmUuZmluYW5jZXMuZ291di5mci9vYXV0aDIvdjEiLCJpYXQiOiJcL0RhdGUoMTQ2ODM2MjU5Mzc4NClcLyJ9";
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream);
            return jsonToken;
        }

        public static string Decode(String st)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(st) as JwtSecurityToken;
            var id = decodedValue.Claims.First(claim => claim.Type == "userid").Value;
            return id;
        }

    }
}