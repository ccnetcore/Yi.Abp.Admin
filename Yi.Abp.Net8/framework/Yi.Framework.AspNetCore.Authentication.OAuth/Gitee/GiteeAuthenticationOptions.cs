/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/aspnet-contrib/AspNet.Security.OAuth.Providers
 * for more information concerning the license and the contributors participating to this project.
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using static Yi.Framework.AspNetCore.Authentication.OAuth.Gitee.GiteeAuthenticationConstants;

namespace Yi.Framework.AspNetCore.Authentication.OAuth.Gitee;

/// <summary>
/// Defines a set of options used by <see cref="GiteeAuthenticationHandler"/>.
/// </summary>
public class GiteeAuthenticationOptions : AuthenticationOAuthOptions
{
    public GiteeAuthenticationOptions()
    {
        ClaimsIssuer = GiteeAuthenticationDefaults.Issuer;

        CallbackPath = GiteeAuthenticationDefaults.CallbackPath;

        AuthorizationEndpoint = GiteeAuthenticationDefaults.AuthorizationEndpoint;
        TokenEndpoint = GiteeAuthenticationDefaults.TokenEndpoint;
        UserInformationEndpoint = GiteeAuthenticationDefaults.UserInformationEndpoint;
        UserEmailsEndpoint = GiteeAuthenticationDefaults.UserEmailsEndpoint;

        Scope.Add("user_info");
        Scope.Add("emails");

        ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
        ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
        ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
        ClaimActions.MapJsonKey(Claims.Url, "url");
    }

    /// <summary>
    /// Gets or sets the address of the endpoint exposing
    /// the email addresses associated with the logged in user.
    /// </summary>
    public string UserEmailsEndpoint { get; set; }
}