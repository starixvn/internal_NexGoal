namespace NexGoal.Core.Attribute
{
    using System.Net.Http;
    using System.Web.Http.Filters;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1018:MarkAttributesWithAttributeUsage", Justification = "Reviewed")]
    public sealed class DeflateCompressionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext != null)
            {
                if (actionExecutedContext.Response != null)
                {
                    var content = actionExecutedContext.Response.Content;
                    var bytes = content == null ? null : content.ReadAsByteArrayAsync().Result;
                    var zlibbedContent = bytes == null
                        ? new byte[0]
                        : CompressionHelper.DeflateByte(bytes);
                    actionExecutedContext.Response.Content = new ByteArrayContent(zlibbedContent);
                    actionExecutedContext.Response.Content.Headers.Remove("Content-Type");
                    actionExecutedContext.Response.Content.Headers.Add("Content-encoding", "deflate");
                    actionExecutedContext.Response.Content.Headers.Add("Content-Type", "application/json");
                    base.OnActionExecuted(actionExecutedContext);
                }
            }
        }
    }
}