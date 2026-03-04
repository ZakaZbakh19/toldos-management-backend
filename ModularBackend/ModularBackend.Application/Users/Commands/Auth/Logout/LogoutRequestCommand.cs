using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Logout
{
    public record LogoutRequestCommand(string refreshRaw) : IRequest<Unit>;
}
