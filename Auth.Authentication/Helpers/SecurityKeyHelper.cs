﻿using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.Authentication.Helpers;

internal static class SecurityKeyHelper
{
    public static SecurityKey Create(string tokenPassword) =>
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenPassword));
}