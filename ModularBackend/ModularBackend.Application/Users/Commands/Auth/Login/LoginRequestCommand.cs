using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Login
{
    public record LoginRequestCommand(string email, string password) : IRequest<AuthResponseCommand>;
}
