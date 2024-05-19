// -----------------------------------------------------------------------
// <copyright file="RedisTicketStore.cs" company="1-system-group">
// Copyright (c) 1-system-group. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using StackExchange.Redis;

namespace Diary_Sample.Infra;

public class RedisTicketStore : ITicketStore
{
    private readonly string keyPrefix = "AuthSessionStore-";
    private readonly IDatabase _cache;

    public RedisTicketStore(IConnectionMultiplexer redis)
    {
        if (redis is null)
        {
            throw new ArgumentNullException(nameof(redis));
        }

        _cache = redis.GetDatabase();
    }

    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        string? key = keyPrefix + Guid.NewGuid().ToString();
        await RenewAsync(key, ticket).ConfigureAwait(false);
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        byte[] val = TicketSerializer.Default.Serialize(ticket);
        await _cache.StringSetAsync(key, val).ConfigureAwait(false);
    }

    public async Task<AuthenticationTicket> RetrieveAsync(string key)
    {
        RedisValue bytes = await _cache.StringGetAsync(key).ConfigureAwait(false);
        AuthenticationTicket? ticket = TicketSerializer.Default.Deserialize(bytes);
        return ticket;
    }

    public async Task RemoveAsync(string key)
    {
        await _cache.KeyDeleteAsync(key).ConfigureAwait(false);
    }
}