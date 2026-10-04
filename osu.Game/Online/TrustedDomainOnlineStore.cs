// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.IO.Stores;
using osu.Framework.Logging;

namespace osu.Game.Online
{
    public sealed class TrustedDomainOnlineStore : OnlineStore
    {
        // Hosts this client is allowed to load resources from. osu!'s own CDN
        // plus our own deployment: the API serves user avatars and beatmap
        // covers, and the upstream *.ppy.sh-only check silently blanked every
        // one of them on this server.
        private static readonly string[] trusted_hosts = { ".ppy.sh", ".041095.xyz" };

        protected override string GetLookupUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            {
                Logger.Log($@"Blocking resource lookup from malformed url: {url}", LoggingTarget.Network, LogLevel.Important);
                return string.Empty;
            }

            foreach (string host in trusted_hosts)
            {
                if (uri.Host.EndsWith(host, StringComparison.OrdinalIgnoreCase))
                    return url;
            }

            Logger.Log($@"Blocking resource lookup from external website: {url}", LoggingTarget.Network, LogLevel.Important);
            return string.Empty;
        }
    }
}
