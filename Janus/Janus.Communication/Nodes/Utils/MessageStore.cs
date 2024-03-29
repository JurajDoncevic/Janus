﻿using Janus.Commons.Messages;
using System.Collections.Concurrent;
using static Janus.Base.Resulting.Results;

namespace Janus.Communication.Nodes.Utils;

/// <summary>
/// Message store used to store relevant received messages. It contains logic for ignoring responses of unknown exchanges.
/// A response might arrive too late to be taken into account, or the node might be spammed with responses if this class were not used.
/// </summary>
public class MessageStore
{
    private readonly ConcurrentDictionary<string, ConcurrentQueue<BaseMessage>> _receivedResponseMessages;
    private readonly ConcurrentDictionary<string, ConcurrentQueue<BaseMessage>> _receivedRequestMessages;
    private readonly ConcurrentDictionary<string, Unit> _registeredExchangeIds; // don't ask, just don't

    /// <summary>
    /// Number of responses that are enqueued thorugh all exchanges
    /// </summary>
    public int CountResponsesEnqueued => _receivedResponseMessages.Values.Sum(q => q.Count);
    /// <summary>
    /// Number of responses that are enqueued thorugh all exchanges
    /// </summary>
    public int CountRequestsEnqueued => _receivedRequestMessages.Values.Sum(q => q.Count);

    /// <summary>
    /// Constructor
    /// </summary>
    public MessageStore()
    {
        _receivedResponseMessages = new ConcurrentDictionary<string, ConcurrentQueue<BaseMessage>>();
        _receivedRequestMessages = new ConcurrentDictionary<string, ConcurrentQueue<BaseMessage>>();
        _registeredExchangeIds = new ConcurrentDictionary<string, Unit>();
    }

    /// <summary>
    /// Registers an exchange so its responses are saved
    /// </summary>
    /// <param name="exchangeId"></param>
    /// <returns>Returns <code>true</code> if the exchange is not already registered, else <code>false</code></returns>
    public bool RegisterExchange(string exchangeId)
    {
        if (string.IsNullOrEmpty(exchangeId))
        {
            throw new ArgumentException($"'{nameof(exchangeId)}' cannot be null or empty.", nameof(exchangeId));
        }

        return _registeredExchangeIds.TryAdd(exchangeId, Unit());
    }

    /// <summary>
    /// Unregisters and exchange so its responses are not saved
    /// </summary>
    /// <param name="exchangeId"></param>
    /// <returns>Returns <code>true</code> if the exchange is unregistered, else <code>false</code></returns>
    public bool UnregisterExchange(string exchangeId)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        return _registeredExchangeIds.TryRemove(exchangeId, out _) && _receivedResponseMessages.ContainsKey(exchangeId) ? _receivedResponseMessages.TryRemove(exchangeId, out _) : true;
    }

    /// <summary>
    /// Determines if the given exchange id is currently registered
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <returns></returns>
    public bool IsRegisteredExchange(string exchangeId)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        return _registeredExchangeIds.ContainsKey(exchangeId);
    }

    /// <summary>
    /// Enqueues a response in an exchange
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <param name="message">Message to enqueue</param>
    /// <returns></returns>
    public bool EnqueueResponseInExchange(string exchangeId, BaseMessage message)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        if (!_registeredExchangeIds.ContainsKey(exchangeId))
            return false;

        // if the exchange already exists
        if (_receivedResponseMessages.ContainsKey(exchangeId))
        {
            // enqueue the new message
            _receivedResponseMessages[exchangeId].Enqueue(message);
            return true;
        }
        else // if the exchange doesn't exist
        {
            if (_receivedResponseMessages.TryAdd(exchangeId, new ConcurrentQueue<BaseMessage>()))
            {
                _receivedResponseMessages[exchangeId].Enqueue(message);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Enqueues a request in an exchange
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <param name="message">Message to enqueue</param>
    /// <returns></returns>
    public bool EnqueueRequestInExchange(string exchangeId, BaseMessage message)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }
        // if the exchange already exists
        if (_receivedRequestMessages.ContainsKey(exchangeId))
        {
            // enqueue the new message
            _receivedRequestMessages[exchangeId].Enqueue(message);
            return true;
        }
        else // if the exchange doesn't exist
        {
            if (_receivedRequestMessages.TryAdd(exchangeId, new ConcurrentQueue<BaseMessage>()))
            {
                _receivedRequestMessages[exchangeId].Enqueue(message);
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if any responses exist in the exchange
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <returns></returns>
    public bool AnyResponsesExist(string exchangeId)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        return _receivedResponseMessages.ContainsKey(exchangeId)
           && !_receivedResponseMessages[exchangeId].IsEmpty;
    }

    /// <summary>
    /// Dequeues a response from the exchange
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Result<BaseMessage> DequeueResponseFromExchange(string exchangeId)
        => AsResult(() =>
        {
            if (AnyResponsesExist(exchangeId) && _receivedResponseMessages[exchangeId].TryDequeue(out var message))
            {
                if (_receivedResponseMessages[exchangeId].IsEmpty)
                {
                    _receivedResponseMessages.Remove(exchangeId, out _);
                }
                return message;
            }
            throw new Exception($"Failed to get response from {exchangeId}");
        });

    /// <summary>
    /// Checks if any request exists in the exchange
    /// </summary>
    /// <param name="exchangeId">Exchange id</param>
    /// <returns></returns>
    public bool AnyRequestsExist(string exchangeId)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        return _receivedRequestMessages.ContainsKey(exchangeId)
       && !_receivedRequestMessages[exchangeId].IsEmpty;
    }

    /// <summary>
    /// Dequeues a request from the exchange
    /// </summary>
    /// <param name="exchangeId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public Result<BaseMessage> DequeueRequestFromExchange(string exchangeId)
    {
        if (exchangeId is null)
        {
            throw new ArgumentNullException(nameof(exchangeId));
        }

        return AsResult(() =>
        {
            if (AnyResponsesExist(exchangeId) && _receivedRequestMessages[exchangeId].TryDequeue(out var message))
            {
                if (_receivedRequestMessages[exchangeId].IsEmpty)
                {
                    _receivedRequestMessages.Remove(exchangeId, out _);
                }
                return message;
            }
            throw new Exception($"Failed to get response from {exchangeId}");
        });
    }
}
