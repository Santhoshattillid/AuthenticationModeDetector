using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Administration.Claims;

namespace AuthenticationModeDetector
{
    internal class Program
    {
        private static void Main()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate
                                                     {
                                                         var farm = SPFarm.Local;

                                                         var service = farm.Services.GetValue<SPWebService>("");

                                                         SPSecurityTokenServiceManager sptMgr = SPSecurityTokenServiceManager.Local;

                                                         Console.WriteLine("list of authentication providers");
                                                         foreach (SPWebApplication spWebApplication in service.WebApplications)
                                                         {
                                                             Console.WriteLine("");
                                                             Console.WriteLine("");
                                                             Console.WriteLine("----------------------------------------");
                                                             Console.WriteLine("Web Application name : " + spWebApplication.Name);
                                                             Console.WriteLine("");
                                                             foreach (KeyValuePair<SPUrlZone, SPIisSettings> spIisSettingse in spWebApplication.IisSettings)
                                                             {
                                                                 Console.WriteLine(spIisSettingse.Key + " : " + spWebApplication.IisSettings[spIisSettingse.Key].AuthenticationMode.ToString());

                                                                 SPIisSettings theSettings = spWebApplication.GetIisSettingsWithFallback(spIisSettingse.Key);

                                                                 //Console.WriteLine("IsTrustedClaimsAuthenticationProvider : " + theSettings.UseTrustedClaimsAuthenticationProvider);

                                                                 if (theSettings.ClaimsAuthenticationProviders != null) //&& theSettings.UseTrustedClaimsAuthenticationProvider
                                                                 {
                                                                     //get the list of authentication providers associated with the zone
                                                                     foreach (SPAuthenticationProvider prov in theSettings.ClaimsAuthenticationProviders)
                                                                     {
                                                                         //get the SPTrustedLoginProvider using the DisplayName
                                                                         SPAuthenticationProvider prov1 = prov;
                                                                         var lp = from SPTrustedLoginProvider spt in sptMgr.TrustedLoginProviders
                                                                                  where spt.DisplayName == prov1.DisplayName
                                                                                  select spt;

                                                                         //there should only be one match, so retrieve that
                                                                         var loginProviders = lp as SPTrustedLoginProvider[] ?? lp.ToArray();
                                                                         if ((loginProviders.Any()))
                                                                         {
                                                                             //get the login provider
                                                                             SPTrustedLoginProvider provider = loginProviders.First();
                                                                             Console.WriteLine("Claims provider name : " + provider.ClaimProviderName);
                                                                         }
                                                                     }
                                                                 }
                                                             }
                                                             Console.WriteLine("----------------------------------------");
                                                         }
                                                         Console.ReadLine();
                                                         Console.ReadKey();
                                                     });
        }
    }
}