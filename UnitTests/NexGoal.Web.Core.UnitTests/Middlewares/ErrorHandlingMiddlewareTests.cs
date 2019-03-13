namespace NexGoal.Web.Core.UnitTests.Middlewares
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Microsoft.AspNetCore.Http;
    using NexGoal.Web.Core.Middlewares;
    using NSubstitute;
    using Xunit;

    public class ErrorHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_DefaultException_RedirectToInternalError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Returns(x => { throw new Exception(); });
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Found, context.Response.StatusCode);
            Assert.Equal("/error/InternalError", context.Response.Headers.Values.First().ToString());
        }

        [Theory]
        [InlineData("post")]
        [InlineData("put")]
        [InlineData("delete")]
        public async Task InvokeAsync_ExceptionAndIsAjaxRequest_ReturnStatus500InternalServerError(string method)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Method = method;
            context.Request.Headers["X-Requested-With"] = "XMLHttpRequest";
            var logger = Substitute.For<ILogger>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Returns(x => { throw new Exception(); });
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_RequestSucceeded_ReturnAsExpected()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger>();
            var next = Substitute.For<RequestDelegate>();
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
}