using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;

namespace Eps.Arm.WebApi2
{
    public static class MyMiddlewareHandler
    {
        public static IAppBuilder UseMyMiddleware(this IAppBuilder app, MyMiddlewareOptions options)
        {
            app.Use<MyMiddleware>(options);
            return app;
        }
    }

    public sealed class MyMiddlewareOptions
    {
        public bool On { get; set; }
        public bool CallNext { get; set; }
    }
}