﻿using Meetups.Domain.Interfaces.Services;
using System;
using System.Text;

namespace Meetups.Services
{
    public class SecurityService : ISecurityService
    {
        public string Encrypt(string texto)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(texto);
            return Convert.ToBase64String(bytes);
        }

        public string Decrypt(string texto)
        {
            byte[] bytes = Convert.FromBase64String(texto);
            return Encoding.Unicode.GetString(bytes);
        }
    }
}
