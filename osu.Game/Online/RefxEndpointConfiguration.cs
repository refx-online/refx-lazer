// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;

namespace osu.Game.Online
{
    /// <summary>
    /// Endpoints for the re;fx private server.
    /// </summary>
    /// <remarks>
    /// Every value can be overridden with an environment variable so that CI builds and local
    /// testing don't require a recompile. The variable name is the property name prefixed with
    /// <c>REFX_</c>, e.g. <c>REFX_API_URL</c>.
    /// </remarks>
    public class RefxEndpointConfiguration : EndpointConfiguration
    {
        private const string env_prefix = "REFX_";

        public RefxEndpointConfiguration()
        {
            // Website is the dorchadas frontend; the API host is where speedforce (osu!web
            // equivalent, API v2 + oauth) is expected to live. mist's /v1 also sits on this
            // host but lazer never talks to /v1.
            WebsiteUrl = env("WEBSITE_URL", "https://refx.041095.xyz");
            APIUrl = env("API_URL", "https://api.041095.xyz");

            // SignalR hubs. Paths must match the hub mappings on the server exactly --
            // these strings are handed straight to HubConnectionBuilder.WithUrl().
            //
            // NOTE: none of these are implemented yet. speedforce has to exist before lazer
            // can authenticate, let alone play multiplayer.
            MultiplayerUrl = env("MULTIPLAYER_URL", $"{APIUrl}/signalr/multiplayer");
            SpectatorUrl = env("SPECTATOR_URL", $"{APIUrl}/signalr/spectator");
            MetadataUrl = env("METADATA_URL", $"{APIUrl}/signalr/metadata");

            // Ranked-play maps only; lazer submits nothing through bss in normal play, so
            // leaving this null keeps us from pointing at a service we don't run.
            BeatmapSubmissionServiceUrl = null;

            // Null disables the whole outage-notification mechanism (see EndpointConfiguration).
            LivenessProbeUrl = null;

            // OAuth client for the re;fx deployment. Must match a row in speedforce's
            // oauth_clients table; the id must also be what the multiplayer hub validates as
            // its JWT audience.
            APIClientID = env("API_CLIENT_ID", "5");

            // Defaulted rather than left empty: the release build bakes this in, and
            // an empty secret makes /oauth/token reject the shipped client with
            // invalid_client, so the game could never log in. Override with
            // REFX_API_CLIENT_SECRET if the server's oauth_clients row differs.
            APIClientSecret = env("API_CLIENT_SECRET", "devsecret");
        }

        private static string env(string name, string fallback) =>
            Environment.GetEnvironmentVariable(env_prefix + name) is { Length: > 0 } value ? value : fallback;
    }
}