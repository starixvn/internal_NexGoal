namespace NexGoal.Web.Core.Middlewares
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Models.Auth;

    public class AccessControlMiddleware
    {
        private readonly RequestDelegate next;
        private readonly AccessControlSettings accessSettings;

        public AccessControlMiddleware(RequestDelegate next, AccessControlSettings accessSettings)
        {
            this.next = next;
            this.accessSettings = accessSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/Whoami"))
            {
                var today = DateTimeOffset.Now.Date;
                var isPublicAccess = accessSettings.PublicDateRanges.Exists(dateRange =>
                {
                    var startDate = DateTime.ParseExact(dateRange.FromDate, "dd/MM", CultureInfo.InvariantCulture);
                    var endDate = DateTime.ParseExact(dateRange.ToDate, "dd/MM", CultureInfo.InvariantCulture);

                    return startDate <= today && today <= endDate;
                });

                if (!isPublicAccess)
                {
                    var clientIP = GetClientIP(context);
                    if (!accessSettings.IPRanges.Exists(ip => IsWhitelistIP(ip, clientIP)))
                    {
                        context.Response.Redirect("/AccessDenied.html");
                        return;
                    }
                }
            }

            await next(context);
        }

        private static IPAddress GetClientIP(HttpContext context)
        {
            var clientIp = context.Request.Headers["X-Forwarded-For"];
            if (!string.IsNullOrWhiteSpace(clientIp))
            {
                var forwardedIps = clientIp.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                clientIp = forwardedIps[0].Trim();
            }
            else
            {
                clientIp = context.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }

            if (clientIp.ToString() == "0.0.0.1")
            {
                clientIp = "127.0.0.1";
            }

            return IPAddress.Parse(clientIp);
        }

        private static bool IsWhitelistIP(string ipRange, IPAddress clientIP)
        {
            return IPNetwork.TryParse(ipRange, out var ipNetwork)
                ? ipNetwork.Contains(clientIP)
                : IsInRange(ipRange, clientIP);
        }

        private static bool IsInRange(string ipRange, IPAddress clientIP)
        {
            IPAddress fromIP;
            IPAddress toIP;
            if (ipRange.Contains('-'))
            {
                var splitPrefix = ipRange.Split('-');
                fromIP = IPAddress.Parse(splitPrefix[0]);
                toIP = IPAddress.Parse(splitPrefix[1]);
            }
            else
            {
                fromIP = IPAddress.Parse(ipRange);
                toIP = IPAddress.Parse(ipRange);
            }

            var ipFrom = BitConverter.ToInt32(fromIP.GetAddressBytes().Reverse().ToArray(), 0);
            var ipEnd = BitConverter.ToInt32(toIP.GetAddressBytes().Reverse().ToArray(), 0);
            var ip = BitConverter.ToInt32(clientIP.GetAddressBytes().Reverse().ToArray(), 0);

            return ip >= ipFrom && ip <= ipEnd;
        }
    }
}
