using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Application.Abstractions.Services.Persistence
{
    public interface IStoreService
    {
        Task<bool> ExistsAsync(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default);
        Task<T?> GetAsync<T>(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default) where T : class;
        Task SaveAsync<T>(string endpoint, string logic, string keyValue, T entity, TimeSpan ttl, CancellationToken cancellationToken = default) where T : class;
        Task DeleteAsync(string endpoint, string logic, string keyValue, CancellationToken cancellationToken = default);
    }
}
