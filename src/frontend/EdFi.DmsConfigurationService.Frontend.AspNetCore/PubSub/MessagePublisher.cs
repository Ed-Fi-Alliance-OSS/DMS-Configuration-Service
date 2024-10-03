// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System.Threading.Tasks;

namespace EdFi.DmsConfigurationService.Frontend.AspNetCore.PubSub
{
    /// <summary>
    /// Message publisher
    /// </summary>
    public class MessagePublisher : IMessagePublisher
    {
        /// <summary>
        /// Publish a message
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="message">Message to publish</param>
        /// <returns>Awaitable</returns>
        public ValueTask PublishAsync<T>(T message)
        {
            // NOTE: For this demo we don't do anything, but a real implementation would publish to Kafka/RabbitMQ/etc

            return ValueTask.CompletedTask;
        }
    }
}
