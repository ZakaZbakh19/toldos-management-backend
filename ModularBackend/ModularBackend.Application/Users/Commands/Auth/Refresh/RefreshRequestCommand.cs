using ModularBackend.Application.Abstractions.Messaging.Mediator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Input;

namespace ModularBackend.Application.Users.Commands.Auth.Refresh
{
    public record RefreshRequestCommand(string refreshRaw) : ICommandRequest<AuthResponseCommand>;
}
