using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth
{
    public record RefreshRequestCommand(string refreshToken) : IRequest<AuthResponseCommand>;
}
