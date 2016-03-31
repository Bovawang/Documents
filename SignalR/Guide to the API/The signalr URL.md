在Startup类中配置SignalR的一般代码如下:
```csharp
using Owin;
using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using SignalR.Notify.Web.App_Start;

[assembly: OwinStartup(typeof(SignalR.Notify.Web.Startup))]
namespace SignalR.Notify.Web
{
    // Use enable signalr
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 设置hub配置
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = false;
            hubConfiguration.EnableJavaScriptProxies = true;

            // 自定义Hubs的pipeline
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule());

            app.MapSignalR("/signalr",hubConfiguration);
        }
    }
}
```

其中`MapSignalR()`方法中的"/signalr"是客户端用来连接你的Hub的route URL.默认情况下就是"/signalr"(不要和客户端的`<script src="signalr/hubs"></script>`自动生成脚本的URL混淆.关于生成代理的更多信息,点击[SignalR Hubs API Guide - JavaScript Client - The generated proxy and what it does for you](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-javascript-client#genproxy)).

可能有特殊情况使SignalR的base URL不可用;例如,在你的项目中有一个名叫signalr的文件夹并且你不想改变这个文件夹的名字.在这种情况下,你可以改变base URL,使用如下的方法:

**Server Code that specifies the URL**

    app.MapSignalR("/signalr", new HubConfiguration());

**JavaScript client code that specified the URL(使用代理)**

```javascript
$.connection.hub.url = "/signalr"
$.connection.hub.start().done(init);
```
**JavaScript client code that specified the URL(不使用代理)**

`var connection = $.hubConnection("/signalr", { useDefaultPath: false });`

**.NET client code that specifies the URL**

`var hubConnection = new HubConnection("http://contoso.com/signalr", useDefault: false);`
