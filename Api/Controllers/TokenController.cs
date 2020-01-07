﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Api.Data;
using Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IConfiguration config;
        private readonly UsersRepository usersRepository;

        public TokenController(IConfiguration config, ApiContext context)
        {
            this.config = config;
            usersRepository = new UsersRepository(context);
        }

        // POST: api/token
        [HttpPost]
        public ActionResult Authenticate(AuthenticationModel auth)
        {
            var user = AuthenticateUser(auth);

            if (user != null)
            {
                var tokenString = GenerateJwt(user);
                return Ok(new { token = tokenString });
            }
            return Unauthorized();
        }

        private string GenerateJwt(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString(), ClaimValueTypes.Integer),
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User AuthenticateUser(AuthenticationModel auth)
        {
            var user = usersRepository.ValidateUser(auth.Username, auth.Password);
            return user;
        }
    }
}