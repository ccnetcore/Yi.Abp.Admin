/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

namespace Yi.Framework.AspNetCore.Authentication.OAuth.Gitee;

/// <summary>
/// Contains constants specific to the <see cref="GiteeAuthenticationHandler"/>.
/// </summary>
public static class GiteeAuthenticationConstants
{
    public static class Claims
    {
        public const string Url = "urn:gitee:url";
        public const string AvatarUrl = "urn:gitee:avatarUrl";
    }
}
