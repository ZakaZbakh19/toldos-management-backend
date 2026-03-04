using Microsoft.AspNetCore.Identity;
using ModularBackend.Application.Abstractions.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Models.Identity
{
    public class Users : IdentityUser, IUsers
    {
    }
}
