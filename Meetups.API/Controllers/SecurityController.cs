﻿using Domain.Exceptions;
using Domain.Models;
using Domain.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Services;
using Services.Logger;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Meetups.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IUserService userService;
        private readonly ISecurityService securityService;
        private readonly ILoggerService logger;

        public SecurityController(IConfiguration config, IUserService userService, ISecurityService securityService, ILoggerService logger)
        {
            this.config = config;
            this.userService = userService;
            this.securityService = securityService;
            this.logger = logger;
        }

        [HttpPost]
        [Route("Login", Name ="Login")]
        public ActionResult Login(LoginRequest loginDetails)
        {
            try
            {
                bool result = ValidateUser(loginDetails);
                if (result)
                {
                    var tokenString = GenerateJWT(loginDetails.User);
                    logger.LogInformation("SecurityController > Login. OK. Token = " + tokenString);
                    return Ok(new { token = tokenString });
                }
                else
                {
                    logger.LogError("SecurityController > Login. ERROR 401");
                    return Unauthorized();
                }
            }
            catch (BadRequestException ex)
            {
                logger.LogError("SecurityController > Login. ERROR 400", ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError("SecurityController > Login. ERROR 500", ex);
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError);
            }
        }

        private string GenerateJWT(string userName)
        {
            var expiry = DateTime.Now.AddHours(4);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
            };

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiry,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private bool ValidateUser(LoginRequest loginDetails)
        {
            loginDetails.Password = securityService.Encrypt(loginDetails.Password);
            return userService.ValidateLogin(loginDetails);
        }
    }
}