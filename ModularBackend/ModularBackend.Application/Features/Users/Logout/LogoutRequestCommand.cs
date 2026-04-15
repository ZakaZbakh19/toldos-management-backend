using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Features.Users.Logout
{
    public record LogoutRequestCommand(string refreshRaw) : ICommandRequest<Unit>;
}
