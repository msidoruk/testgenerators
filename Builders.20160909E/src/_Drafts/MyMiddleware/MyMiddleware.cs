using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using Castle.Core.Internal;
using Microsoft.Owin;
using Microsoft.Owin.Security;

namespace Eps.Arm.WebApi2
{
    public class MyMiddleware : OwinMiddleware
    {
        private readonly MyMiddlewareOptions _options;

        public MyMiddleware(OwinMiddleware next, MyMiddlewareOptions options) : base(next)
        {
            _options = options;
        }

        public string CalculateMd5Hash(string input)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(input));
            return hash.Aggregate(string.Empty, (resultString, currentByte) => resultString + currentByte.ToString("X2"));
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (_options.On)
            {
                // context.Response.StatusCode = 418;
                string[] bearerValues;
                if (context.Request.Headers.TryGetValue("Authorization", out bearerValues) && bearerValues != null)
                {
                    string bearerValue = string.Concat(bearerValues);
                    //
                    var userSession = GetSession(bearerValue);
                    //
                    HttpContext.Current.GetOwinContext().Set("userSession", userSession);
                }
            }
            if (_options.CallNext)
            {
                await Next.Invoke(context);
            }
        }

        private Dictionary<string, object> GetSession(string bearerValue)
        {
            string bearerMd5 = CalculateMd5Hash(bearerValue);
            var sessionsDictionary = GetSessionsDictionary();
            Dictionary<string, object> userSession;
            if (!sessionsDictionary.TryGetValue($"{bearerMd5}", out userSession) || userSession == null)
            {
                userSession = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
                userSession["id"] = Guid.NewGuid();
                userSession["owner"] = bearerValue;
                userSession["owner.short"] = bearerMd5;
                sessionsDictionary[bearerMd5] = userSession;
            }
            return userSession;
        }

        private ConcurrentDictionary<string, Dictionary<string, object>> GetSessionsDictionary()
        {
            var usersSessions = HttpContext.Current.Application["usersSessions"] as ConcurrentDictionary<string, Dictionary<string, object>>;
            if (usersSessions == null)
            {
                usersSessions = new ConcurrentDictionary<string, Dictionary<string, object>>();
                HttpContext.Current.Application["usersSessions"] = usersSessions;
            }
            return usersSessions;
        }
    }
}