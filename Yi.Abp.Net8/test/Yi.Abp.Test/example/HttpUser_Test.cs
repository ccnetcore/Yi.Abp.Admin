using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace Yi.Abp.Test.example
{
    public class HttpUser_Test : YiAbpTestWebBase
    {
        [Fact]
        public void Http_Test()
        {
            var httpContext = GetRequiredService<IHttpContextAccessor>();
            httpContext.HttpContext.Request.Path.ToString().ShouldBe("/test");
        }
    }
}
