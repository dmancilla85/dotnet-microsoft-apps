// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

// <SettingsSnippet>
using Microsoft.Extensions.Configuration;

namespace GraphTutorial;

public class Settings
{
  public string ClientId { get; set; }
  public string ClientSecret { get; set; }
  public string TenantId { get; set; }
  public string AuthTenant { get; set; }
  public string[] GraphUserScopes { get; set; }

  public Settings()
  {
    ClientId = "";
    ClientSecret = "";
    TenantId = "";
    AuthTenant = "";
    GraphUserScopes = Array.Empty<string>();
  }

  public static Settings? LoadSettings()
  {
    // Load settings
    IConfiguration config = new ConfigurationBuilder()
        // appsettings.json is required
        .AddJsonFile("appsettings.json", optional: false)
        // appsettings.Development.json" is optional, values override appsettings.json
        .AddJsonFile($"appsettings.Development.json", optional: true)
        // User secrets are optional, values override both JSON files
        .AddUserSecrets<Program>()
        .Build();

    return config.GetRequiredSection("Settings").Get<Settings>();
  }
}

// </SettingsSnippet>