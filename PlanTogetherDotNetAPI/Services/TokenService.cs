﻿using Microsoft.IdentityModel.Tokens;
using PlanTogetherDotNetAPI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace PlanTogetherDotNetAPI.Services
{
    public class TokenService
    {
        private readonly string _tokenKey;
        private readonly TimeSpan _tokenLifespan;
        private readonly SigningCredentials _signingCredentials;

        public TokenService()
        {
            _tokenKey = ConfigurationManager.AppSettings["TokenKey"];
            _tokenLifespan = TimeSpan.FromHours(1);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenKey));
            _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        }
        public string CreateToken(AppUser user, IEnumerable<string> roles = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };
            if (roles != null)
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(_tokenLifespan),
                SigningCredentials = _signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}