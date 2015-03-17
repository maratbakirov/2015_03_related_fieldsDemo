using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace DemoApp.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            ReadSettings();

            using (ClientContext ctx = GetAuthenticatedContext())
            {
                TraceHelper.TraceInformation(ConsoleColor.Magenta, "reading related items");

            }

        }




        #region support code
        static bool sharepointonline;

        private static bool ReadSettings()
        {
            var sharepointonlinesetting = ConfigurationManager.AppSettings["SharepointOnline"];
            bool.TryParse(sharepointonlinesetting, out sharepointonline);
            return sharepointonline;
        }

        #region auth
        private static ClientContext GetAuthenticatedContext()
        {
            var siteUrl = ConfigurationManager.AppSettings["siteurl"];
            var context = new ClientContext(siteUrl);
            if (sharepointonline)
            {
                SecureString password = GetPassword();
                context.Credentials = new SharePointOnlineCredentials(ConfigurationManager.AppSettings["sharepointonlinelogin"],
                    password);
            }
            return context;
        }

        private static SecureString storedPassword = null;
        private static SecureString GetPassword()
        {
            if (storedPassword == null)
            {
                Console.WriteLine("Please enter your password");
                storedPassword = GetConsoleSecurePassword();
                Console.WriteLine();
            }
            return storedPassword;
        }


        private static SecureString GetConsoleSecurePassword()
        {
            SecureString pwd = new SecureString();
            while (true)
            {
                ConsoleKeyInfo i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.RemoveAt(pwd.Length - 1);
                    }
                    Console.Write("\b \b");
                }
                else
                {
                    pwd.AppendChar(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd;
        }


        #endregion
        #endregion

    }

}
