using Microsoft.Extensions.Logging;
using ModularBackend.Application.Abstractions.Persistence;
using ModularBackend.Application.Mediator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ModularBackend.Application.Behaviors
{
    public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ITransactionalCommandRequest<TResponse>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransactionBehavior(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TResponse> Handle(TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            if (_unitOfWork.HasActiveTransaction)
                return await next();

            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                var response = await next();

                await _unitOfWork.CommitAsync(cancellationToken);

                return response;
            }
            catch
            {
                await _unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
