namespace NexGoal.Site
{
    using System;
    using System.Web;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.SessionState;
    using Fanex.Data;
    using Fanex.Logging;
    using NexGoal.Core.Utilities;

    public class WebApiApplication : HttpApplication
    {
        private const string WebApiPrefix = "api";
        private static readonly string WebApiExecutionPath = string.Format("~/{0}", WebApiPrefix);

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbSettingProviderManager
            .StartNewSession()
            .Use(new CustomConnectionStringProvider())
            .UseDefaultDbSettingProvider(
                resourcePath: "~/App_Data",
                ignoreRedundantParameters: false,
                enableWatching: true)
            .Run();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError().GetBaseException();
            string message = ex.Message +
                    "\r\n Source: " + ex.Source +
                    "\r\n Domain: " + HttpContext.Current.Request.Headers["Host"] +
                    "\r\n TargetSite: " + ex.TargetSite +
                    "\r\n StackTrace: " + ex.StackTrace;

            LogService.Error(message);
        }

        public void Session_Start(object sender, EventArgs e)
        {
            string sessionId = Session.SessionID;
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null
                && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiExecutionPath, StringComparison.OrdinalIgnoreCase);
        }
    }
}