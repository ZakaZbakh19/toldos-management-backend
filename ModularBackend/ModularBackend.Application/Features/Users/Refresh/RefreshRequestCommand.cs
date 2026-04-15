using ModularBackend.Application.Features.Users;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Input;

namespace ModularBackend.Application.Features.Users.Refresh
{
    public record RefreshRequestCommand(string refreshRaw) : ICommandRequest<AuthResponseDTO>;
}
