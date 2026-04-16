using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Messaging
{
    public static class MessagingTopology
    {
        public const string ProductsQueue = "catalog-products";
        public const string ProductsQueueRetry = "catalog-products-retry";
        public const string ProductsQueueDeadLetter = "catalog-products-dlx";
        public const string ProductsConsumer = "catalog-products-consumer";

        public const string RetryExchange = "integration-events-retry";
        public const string DeadLetterExchange = "integration-events-dlx";
    }
}
