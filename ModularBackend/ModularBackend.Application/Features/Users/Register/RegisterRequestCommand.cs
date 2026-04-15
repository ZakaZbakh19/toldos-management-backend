using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Users.Register
{
    public record RegisterRequestCommand(string name, string email, string password) : ICommandRequest<AuthResponseDTO>;
}
