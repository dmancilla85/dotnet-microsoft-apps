// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using Azure.Core;
using Azure.Identity;
using Microsoft.Graph;

namespace GraphTutorial;

internal static class GraphHelper
{
  #region User-auth

  // <UserAuthConfigSnippet>
  // Settings object
  private static Settings _settings = new();

  // User auth token credential
  private static DeviceCodeCredential? _deviceCodeCredential;

  // Client configured with user authentication
  private static GraphServiceClient? _userClient;

  public static void InitializeGraphForUserAuth(Settings settings,
      Func<DeviceCodeInfo, CancellationToken, Task> deviceCodePrompt)
  {
    _settings = settings;

    _deviceCodeCredential = new DeviceCodeCredential(deviceCodePrompt,
        settings.AuthTenant, settings.ClientId);

    _userClient = new GraphServiceClient(_deviceCodeCredential, settings.GraphUserScopes);
  }

  // </UserAuthConfigSnippet>

  // <GetUserTokenSnippet>
  public static async Task<string> GetUserTokenAsync()
  {
    // Ensure credential isn't null
    _ = _deviceCodeCredential ??
        throw new System.NullReferenceException("Graph has not been initialized for user auth");

    // Ensure scopes isn't null

    // Request token with given scopes
    TokenRequestContext context = new TokenRequestContext(_settings.GraphUserScopes);
    AccessToken response = await _deviceCodeCredential.GetTokenAsync(context);
    return response.Token;
  }

  // </GetUserTokenSnippet>

  // <GetUserSnippet>
  public static Task<User> GetUserAsync()
  {
    // Ensure client isn't null
    _ = _userClient ??
        throw new System.NullReferenceException("Graph has not been initialized for user auth");

    return _userClient.Me
        .Request()
        .Select(u => new
        {
          // Only request specific properties
          u.DisplayName,
          u.Mail,
          u.UserPrincipalName
        })
        .GetAsync();
  }

  // </GetUserSnippet>

  // <GetInboxSnippet>
  public static Task<IMailFolderMessagesCollectionPage> GetInboxAsync()
  {
    // Ensure client isn't null
    _ = _userClient ??
        throw new System.NullReferenceException("Graph has not been initialized for user auth");

    return _userClient.Me
        // Only messages from Inbox folder
        .MailFolders["Inbox"]
        .Messages
        .Request()
        .Select(m => new
        {
          // Only request specific properties
          m.From,
          m.IsRead,
          m.ReceivedDateTime,
          m.Subject
        })
        // Get at most 25 results
        .Top(25)
        // Sort by received time, newest first
        .OrderBy("ReceivedDateTime DESC")
        .GetAsync();
  }

  // </GetInboxSnippet>

  // <SendMailSnippet>
  public static async Task SendMailAsync(string subject, string body, string recipient)
  {
    // Ensure client isn't null
    _ = _userClient ??
        throw new System.NullReferenceException("Graph has not been initialized for user auth");

    // Create a new message
    Message message = new Message
    {
      Subject = subject,
      Body = new ItemBody
      {
        Content = body,
        ContentType = BodyType.Text
      },
      ToRecipients = new Recipient[]
        {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = recipient
                    }
                }
        }
    };

    // Send the message
    await _userClient.Me
        .SendMail(message)
        .Request()
        .PostAsync();
  }

  // </SendMailSnippet>

  #endregion User-auth

  #region App-only

  // <AppOnyAuthConfigSnippet>
  // App-ony auth token credential
  private static ClientSecretCredential? _clientSecretCredential;

  // Client configured with app-only authentication
  private static GraphServiceClient? _appClient;

  private static void EnsureGraphForAppOnlyAuth()
  {
    // Ensure settings isn't null
    _ = _settings ??
        throw new System.NullReferenceException("Settings cannot be null");

    _clientSecretCredential ??= new ClientSecretCredential(
          _settings.TenantId, _settings.ClientId, _settings.ClientSecret);

    _appClient ??= new GraphServiceClient(_clientSecretCredential,
          // Use the default scope, which will request the scopes
          // configured on the app registration
          new[] { "https://graph.microsoft.com/.default" });
  }

  // </AppOnyAuthConfigSnippet>

  // <GetUsersSnippet>
  public static Task<IGraphServiceUsersCollectionPage> GetUsersAsync()
  {
    EnsureGraphForAppOnlyAuth();
    // Ensure client isn't null
    _ = _appClient ??
        throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

    return _appClient.Users
        .Request()
        .Select(u => new
        {
          // Only request specific properties
          u.DisplayName,
          u.Id,
          u.Mail
        })
        // Get at most 25 results
        .Top(25)
        // Sort by display name
        .OrderBy("DisplayName")
        .GetAsync();
  }

  // </GetUsersSnippet>

  #endregion App-only

#pragma warning disable CS1998

  // <MakeGraphCallSnippet>
  // This function serves as a playground for testing Graph snippets
  // or other code
  public static async Task<IOnenoteNotebooksCollectionPage> MakeGraphCallAsync()
  {
    // Note: if using _appClient, be sure to call EnsureGraphForAppOnlyAuth
    // before using it.
    // EnsureGraphForAppOnlyAuth
    try
    {
      // Ensure client isn't null
      _ = _userClient ??
          throw new System.NullReferenceException("Graph has not been initialized for app-only auth");

      return await _userClient.Me.Onenote.Notebooks
      .Request()
      .GetAsync();
    }
    catch (Exception ex)
    {
      Console.WriteLine($"Something has happened: {ex.Message}");
      return new OnenoteNotebooksCollectionPage();
    }
  }

  // </MakeGraphCallSnippet>
}