//
//  CachingDiscordRestEmojiAPI.cs
//
//  Author:
//       Jarl Gullberg <jarl.gullberg@gmail.com>
//
//  Copyright (c) 2017 Jarl Gullberg
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Caching.Abstractions.Services;
using Remora.Discord.Caching.Services;
using Remora.Discord.Rest.API;
using Remora.Rest;
using Remora.Rest.Core;
using Remora.Results;

namespace Remora.Discord.Caching.API;

/// <summary>
/// Implements a caching version of the channel API.
/// </summary>
[PublicAPI]
public class CachingDiscordRestEmojiAPI : DiscordRestEmojiAPI
{
    private readonly CacheService _cacheService;

    /// <inheritdoc cref="DiscordRestEmojiAPI(IRestHttpClient, JsonSerializerOptions, ICacheProvider)" />
    public CachingDiscordRestEmojiAPI
    (
        IRestHttpClient restHttpClient,
        JsonSerializerOptions jsonOptions,
        ICacheProvider rateLimitCache,
        CacheService cacheService
    )
        : base(restHttpClient, jsonOptions, rateLimitCache)
    {
        _cacheService = cacheService;
    }

    /// <inheritdoc />
    public override async Task<Result<IEmoji>> GetGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateEmojiCacheKey(guildID, emojiID);
        var cacheResult = await _cacheService.TryGetValueAsync<IEmoji>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return Result<IEmoji>.FromSuccess(cacheResult.Entity);
        }

        var getResult = await base.GetGuildEmojiAsync(guildID, emojiID, ct);
        if (!getResult.IsSuccess)
        {
            return getResult;
        }

        var emoji = getResult.Entity;
        await _cacheService.CacheAsync(key, emoji, ct);

        return getResult;
    }

    /// <inheritdoc />
    public override async Task<Result<IEmoji>> CreateGuildEmojiAsync
    (
        Snowflake guildID,
        string name,
        Stream image,
        IReadOnlyList<Snowflake> roles,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var createResult = await base.CreateGuildEmojiAsync(guildID, name, image, roles, reason, ct);
        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var emoji = createResult.Entity;
        if (emoji.ID is null)
        {
            // We can't, or shouldn't, cache this
            return createResult;
        }

        var key = KeyHelpers.CreateEmojiCacheKey(guildID, emoji.ID.Value);
        await _cacheService.CacheAsync(key, emoji, ct);

        return createResult;
    }

    /// <inheritdoc />
    public override async Task<Result<IEmoji>> ModifyGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        Optional<string> name = default,
        Optional<IReadOnlyList<Snowflake>?> roles = default,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var modifyResult = await base.ModifyGuildEmojiAsync(guildID, emojiID, name, roles, reason, ct);
        if (!modifyResult.IsSuccess)
        {
            return modifyResult;
        }

        var emoji = modifyResult.Entity;
        var key = KeyHelpers.CreateEmojiCacheKey(guildID, emojiID);
        await _cacheService.CacheAsync(key, emoji, ct);

        return modifyResult;
    }

    /// <inheritdoc />
    public override async Task<Result> DeleteGuildEmojiAsync
    (
        Snowflake guildID,
        Snowflake emojiID,
        Optional<string> reason = default,
        CancellationToken ct = default
    )
    {
        var deleteResult = await base.DeleteGuildEmojiAsync(guildID, emojiID, reason, ct);
        if (!deleteResult.IsSuccess)
        {
            return deleteResult;
        }

        var key = KeyHelpers.CreateEmojiCacheKey(guildID, emojiID);
        await _cacheService.EvictAsync<IEmoji>(key, ct);

        return deleteResult;
    }

    /// <inheritdoc />
    public override async Task<Result<IReadOnlyList<IEmoji>>> ListGuildEmojisAsync
    (
        Snowflake guildID,
        CancellationToken ct = default
    )
    {
        var key = KeyHelpers.CreateGuildEmojisCacheKey(guildID);
        var cacheResult = await _cacheService.TryGetValueAsync<IReadOnlyList<IEmoji>>(key, ct);

        if (cacheResult.IsSuccess)
        {
            return Result<IReadOnlyList<IEmoji>>.FromSuccess(cacheResult.Entity);
        }

        var result = await base.ListGuildEmojisAsync(guildID, ct);
        if (!result.IsSuccess)
        {
            return result;
        }

        foreach (var emoji in result.Entity)
        {
            if (emoji.ID is null)
            {
                continue;
            }

            var emojiKey = KeyHelpers.CreateEmojiCacheKey(guildID, emoji.ID.Value);
            await _cacheService.CacheAsync(emojiKey, emoji, ct);
        }

        return result;
    }
}
