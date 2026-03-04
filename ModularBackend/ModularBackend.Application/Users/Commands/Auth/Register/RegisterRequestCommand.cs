using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Register
{
    public record RegisterRequestCommand(string name, string email, string password) : IRequest<AuthResponseCommand>;
}
