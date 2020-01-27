﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Consumer;

namespace Azure.Messaging.ServiceBus.Core
{
    /// <summary>
    ///   Provides an abstraction for generalizing an Event Hub client so that a dedicated instance may provide operations
    ///   for a specific transport, such as AMQP or JMS.  It is intended that the public <see cref="ServiceBusConnection" /> employ
    ///   a transport client via containment and delegate operations to it rather than understanding protocol-specific details
    ///   for different transports.
    /// </summary>
    ///
    internal abstract class TransportClient : IAsyncDisposable
    {
        /// <summary>
        ///   Indicates whether or not this client has been closed.
        ///   </summary>
        ///
        /// <value>
        ///   <c>true</c> if the client is closed; otherwise, <c>false</c>.
        /// </value>
        ///
        public virtual bool IsClosed { get; }

        /// <summary>
        ///   The endpoint for the Event Hubs service to which the client is associated.
        /// </summary>
        ///
        public virtual Uri ServiceEndpoint { get; }

        /// <summary>
        ///   Retrieves information about an Event Hub, including the number of partitions present
        ///   and their identifiers.
        /// </summary>
        ///
        /// <param name="retryPolicy">The retry policy to use as the basis for retrieving the information.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
        ///
        /// <returns>The set of information for the Event Hub that this client is associated with.</returns>
        ///
        public abstract Task<EventHubProperties> GetPropertiesAsync(ServiceBusRetryPolicy retryPolicy,
                                                                    CancellationToken cancellationToken);

        /// <summary>
        ///   Retrieves information about a specific partition for an Event Hub, including elements that describe the available
        ///   events in the partition event stream.
        /// </summary>
        ///
        /// <param name="partitionId">The unique identifier of a partition associated with the Event Hub.</param>
        /// <param name="retryPolicy">The retry policy to use as the basis for retrieving the information.</param>
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
        ///
        /// <returns>The set of information for the requested partition under the Event Hub this client is associated with.</returns>
        ///
        public abstract Task<PartitionProperties> GetPartitionPropertiesAsync(string partitionId,
                                                                              ServiceBusRetryPolicy retryPolicy,
                                                                              CancellationToken cancellationToken);

        /// <summary>
        ///
        /// </summary>
        /// <param name="consumer"></param>
        /// <param name="retryPolicy"></param>
        /// <param name="fromSequenceNumber"></param>
        /// <param name="messageCount"></param>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public abstract Task<IEnumerable<ServiceBusMessage>> PeekAsync(
            TransportConsumer consumer,
            ServiceBusRetryPolicy retryPolicy,
            long fromSequenceNumber,
            int messageCount = 1,
            string sessionId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///   Creates a producer strongly aligned with the active protocol and transport,
        ///   responsible for publishing <see cref="EventData" /> to the Event Hub.
        /// </summary>
        ///
        /// <param name="partitionId">The identifier of the partition to which the transport producer should be bound; if <c>null</c>, the producer is unbound.</param>
        /// <param name="retryPolicy">The policy which governs retry behavior and try timeouts.</param>
        ///
        /// <returns>A <see cref="TransportSender"/> configured in the requested manner.</returns>
        ///
        public abstract TransportSender CreateSender(string partitionId,
                                                         ServiceBusRetryPolicy retryPolicy);

        /// <summary>
        ///   Creates a consumer strongly aligned with the active protocol and transport, responsible
        ///   for reading <see cref="EventData" /> from a specific Event Hub partition, in the context
        ///   of a specific consumer group.
        ///
        ///   A consumer may be exclusive, which asserts ownership over the partition for the consumer
        ///   group to ensure that only one consumer from that group is reading the from the partition.
        ///   These exclusive consumers are sometimes referred to as "Epoch Consumers."
        ///
        ///   A consumer may also be non-exclusive, allowing multiple consumers from the same consumer
        ///   group to be actively reading events from the partition.  These non-exclusive consumers are
        ///   sometimes referred to as "Non-epoch Consumers."
        ///
        ///   Designating a consumer as exclusive may be specified by setting the <paramref name="ownerLevel" />.
        ///   When <c>null</c>, consumers are created as non-exclusive.
        /// </summary>
        ///
        /// <param name="eventPosition">The position within the partition where the consumer should begin reading events.</param>
        /// <param name="retryPolicy">The policy which governs retry behavior and try timeouts.</param>
        /// <param name="trackLastEnqueuedEventProperties">Indicates whether information on the last enqueued event on the partition is sent as events are received.</param>
        /// <param name="ownerLevel">The relative priority to associate with the link; for a non-exclusive link, this value should be <c>null</c>.</param>
        /// <param name="prefetchCount">Controls the number of events received and queued locally without regard to whether an operation was requested.  If <c>null</c> a default will be used.</param>
        /// <param name="sessionId"></param>
        ///
        /// <returns>A <see cref="TransportConsumer" /> configured in the requested manner.</returns>
        ///
        public abstract TransportConsumer CreateConsumer(
            //string consumerGroup,
            //                                             string partitionId,
                                                         EventPosition eventPosition,
                                                         ServiceBusRetryPolicy retryPolicy,
                                                         bool trackLastEnqueuedEventProperties,
                                                         long? ownerLevel,
                                                         uint? prefetchCount,
                                                         string sessionId);

        /// <summary>
        ///   Closes the connection to the transport client instance.
        /// </summary>
        ///
        /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> instance to signal the request to cancel the operation.</param>
        ///
        public abstract Task CloseAsync(CancellationToken cancellationToken);

        /// <summary>
        ///   Performs the task needed to clean up resources used by the client,
        ///   including ensuring that the client itself has been closed.
        /// </summary>
        ///
        /// <returns>A task to be resolved on when the operation has completed.</returns>
        ///
        public virtual async ValueTask DisposeAsync() => await CloseAsync(CancellationToken.None).ConfigureAwait(false);
    }
}