using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.WebPages.OAuth;
using MegaFilesSharing.Models;

namespace MegaFilesSharing
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

           // OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "000000004C11484D",
            //    clientSecret: "ZGNZGJCdymxvbnBNF4VbmZ3lApzl35oU");

            OAuthWebSecurity.RegisterTwitterClient(
                consumerKey: "5msjIrM4CdF5Q5iRE2Y8b40fF",
                consumerSecret: "Q3ORSTYEPvecNxILAegWja9q9KUkpApwZR7awBFjY2WhdpmhCD");

            OAuthWebSecurity.RegisterFacebookClient(
                appId: "485694728198607",
                appSecret: "dde1f34fe3815cfb459d65e9613913c8");

            OAuthWebSecurity.RegisterGoogleClient();
        }
    }
}
