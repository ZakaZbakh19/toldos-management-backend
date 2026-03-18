using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Services
{
    public sealed record FileStoredResult(
        string Url,
        string StorageKey);
}
