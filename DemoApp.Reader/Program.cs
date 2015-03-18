using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using DemoApp.Model;
using Newtonsoft.Json;
using SPMeta2.CSOM.DefaultSyntax;
using SPMeta2.CSOM.Utils;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;

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

                ctx.Load(ctx.Web, x=>x.ServerRelativeUrl);
                ctx.ExecuteQuery();

                var workflowTasksListUrl = UrlUtility.CombineUrl(ctx.Web.ServerRelativeUrl, Lists.WorkflowTasks.GetListUrl());
                var workflowTasksLIst = ctx.Web.GetList(workflowTasksListUrl);

                // this is just for demo, this is not the best code to work with sharpeoint items
                var items = workflowTasksLIst.GetItems(
                    new CamlQuery()
                    );

                ctx.Load(items);

                ctx.ExecuteQuery();

                string relatedItemsString = (string)items[0][BuiltInInternalFieldNames.RelatedItems];

                dynamic decodedRelatedItems = JsonConvert.DeserializeObject(relatedItemsString);
                foreach (var item in decodedRelatedItems)
                {
                    int itemId = int.Parse(item.ItemId.ToString());
                    var listId = new Guid(string.Format("{{{0}}}", item.ListId.ToString()));
                    var webId = new Guid(string.Format("{{{0}}}", item.WebId.ToString()));

                    Console.WriteLine("Found an item from web {0}, list {1}, itemid:{2}",webId,listId,itemId);
                    
                }





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
