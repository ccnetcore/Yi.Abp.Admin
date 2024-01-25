using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shouldly;
using Xunit;

namespace Yi.Abp.Test.Demo
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
