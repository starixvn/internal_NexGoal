namespace NexGoal.Web.Core.Middlewares
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Models.Auth;

    public class AccessControlMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AccessSettings accessSettings;

        public AccessControlMiddleware(RequestDelegate next, AccessSettings accessSettings)
        {
            this.next = next;
            this.accessSettings = accessSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/Error"))
            {
                var today = DateTime.Now.Date;
                var isPublicAccess = accessSettings.DateRanges.Exists(dateRange =>
                {
                    var startDate = DateTime.ParseExact(dateRange.StartDate, "dd/MM", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(dateRange.EndDate, "dd/MM", CultureInfo.InvariantCulture);

                    return startDate <= today && today <= endDate;
                });

                if (!isPublicAccess)
                {
                    var ipAddress = GetClientIP(context);
                    var isAllowed = accessSettings.IPRanges.Exists(range => IPNetwork.Parse(range).Contains(ipAddress));

                    if (!isAllowed)
                    {
                        context.Response.Redirect("/Error/AccessDenied");
                        return;
                    }
                }
            }
            
            await next(context);
        }

        private IPAddress GetClientIP(HttpContext context)
        {
            var clientIp = context.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrEmpty(clientIp))
            {
                var forwardedIps = clientIp.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                clientIp = forwardedIps[0].Trim();
            }
            else
            {
                clientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            if (clientIp.ToString() == "::1")
            {
                clientIp = "127.0.0.1";
            }

            return IPAddress.Parse(clientIp);
        }
    }
}
