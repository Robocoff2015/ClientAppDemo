using System;
using System.IO;
using System.Globalization;
using Microsoft.Extensions.Configuration;
namespace ClientApp
{
    public class AuthConfig
  {
    public string Azure_Instance {get; set;} =
      "https://login.microsoftonline.com/{0}";
    public string Scada_TenantId {get; set;}
    public string Registered_ClientId {get; set;}
    public string Authority
    {
      get
      {
        return String.Format(CultureInfo.InvariantCulture, 
                             Azure_Instance, Scada_TenantId);
      }
    }
    public string Scada_ClientSecret {get; set;}
    public string Scada_BaseAddress {get; set;}
    public string Scada_ResourceId {get; set;}

    public static AuthConfig ReadFromJsonFile(string path)
    {
      IConfiguration Configuration;

      var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile(path);

      Configuration = builder.Build();

      return Configuration.Get<AuthConfig>();
    }
  }


}