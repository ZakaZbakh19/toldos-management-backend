using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Users.Login
{
    public record LoginRequestCommand(string email, string password) : ICommandRequest<AuthResponseDTO>;
}
