using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Users.Commands.Auth.Refresh
{
    public record RefreshRequestCommand(string refreshRaw) : IRequest<AuthResponseCommand>;
}
