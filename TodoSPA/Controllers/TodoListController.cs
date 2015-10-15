using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Security.Claims;
using TodoSPA.DAL;
using System.Configuration;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SharePoint.Client;
//using System.Configuration;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web.Http;

namespace TodoSPA.Controllers
{

    // [Authorize]
    public class TodoListController : ApiController
    {
        private TodoListServiceContext db = new TodoListServiceContext();

        // GET: api/TodoList
        public IHttpActionResult Get()
        {
            string sharePointUrl = "https://appliancedirectclan.sharepoint.com";
            string newToken = GetSharePointAccessToken(sharePointUrl, this.Request.Headers.Authorization.Parameter);
            using (ClientContext clientContext = new ClientContext(sharePointUrl))
            {

                /// Adding authorization header 
                clientContext.ExecutingWebRequest += (s, e) => e.WebRequestExecutor.WebRequest.Headers.Add("Authorization", "Bearer " + this.Request.Headers.Authorization.Parameter);

                Web web = clientContext.Web;
                clientContext.Load(web);

                clientContext.ExecuteQuery();

                List listObject = clientContext.Web.Lists.GetByTitle("Documents");

                clientContext.ExecuteQuery();

                CamlQuery caml = new CamlQuery();
                caml.ViewXml = "<View><ViewFields><FieldRef Name='FileLeafRef' /></ViewFields><Query><Where><Contains><FieldRef Name='FileLeafRef'/><Value Type='File'>" + "1" + "</Value></Contains></Where></Query><RowLimit>100</RowLimit></View>";
                //caml.ViewXml = "<View><Query><Where><Geq><FieldRef Name='ID'/><Value Type='Number'>10</Value></Geq></Where></Query><RowLimit>100</RowLimit></View>";

                ListItemCollection items = listObject.GetItems(caml);


                clientContext.Load(items);

                clientContext.ExecuteQuery();

                List<DocOut> outs = new List<DocOut>();
                foreach (ListItem item in items)
                {
                    DocOut outt = new DocOut();
                    outt.name = item["FileLeafRef"].ToString();
                    outt.id = item.Id;
                    outs.Add(outt);
                }

                return Ok(outs);
            }
        }


        internal static string GetSharePointAccessToken(string url, string accessToken)
        {
            string clientID = ConfigurationManager.AppSettings["ida:Audience"];
            string clientSecret = ConfigurationManager.AppSettings["ClientSecret"];

            var appCred = new ClientCredential(clientID, clientSecret);
            var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext("https://login.windows.net/common");

            AuthenticationResult authResult = authContext.AcquireToken(new Uri(url).GetLeftPart(UriPartial.Authority), appCred, new UserAssertion(accessToken));
            return authResult.AccessToken;
        }
    }
}
