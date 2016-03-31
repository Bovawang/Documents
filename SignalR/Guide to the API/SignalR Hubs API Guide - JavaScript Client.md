## How to establish a connection<a id='howtoestablishaconnection'></a>
在你建立连接之前,必须常见一个连接对象,创建一个代理,为在服务端调用的函数注册event handlers.当代理和event handlers准备完,通过调用`start()`方法建立连接.
如果你使用生成代理的方式,在代码中就不需要创建连接对象,因为生成代理的代码帮你去做.

　**建立一个连接(使用代理)**

```JavaScript
var contosoChatHubProxy = $.connection.contosoChatHub;
contosoChatHubProxy.client.addContosoChatMessageToPage = function (name, message) {
    console.log(userName + ' ' + message);
};
$.connection.hub.start()
    .done(function(){ console.log('Now connected, connection ID=' + $.connection.hub.id); })
    .fail(function(){ console.log('Could not Connect!'); });
});
```

　**建立一个连接(使用代理)**

```JavaScript
// 创建连接对象
var connection = $.hubConnection();
// 创建代理
var chatHubProxy = connection.createHubProxy('chatHub');
// 注册服务端函数调用的event handlers(可以多个)
chatHubProxy.on('hello', function(userName, message){
    console.log(userName+' '+message);
});
// 开始建立连接
connection.start().done(function(){
    console.log('Now connected, connected ID='+connection.id);
}).fail(function(){
    console.log('Could not connect');
});
```
这段代码使用默认的"/signalr" URL连接SignalR服务端.关于如何制定一个不同的base URL信息,请看[ASP.NET SignalR Hubs API Guide - Server - The /signalr URL](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-server#signalrurl).
默认,hub地址是当前server;如果你想连接到不同的服务端,在调用start方法前指定URL,如下:
`$.connection.hub.url = '<yourbackendurl>';`
> Note: 通常要在调用start方法建立连接之前注册event handlers.如果你想在连接之后注册一些event handlers,你可以那么做,但是你必须在调用start方法之前注册event handlers中的一个或多个.一个原因是应用中可能有多个Hubs,但是你只会用到其中的一个而不想触发每一个Hub的`OnConnected`事件.当连接建立之后,Hub代理上的方法是否存在是通过告诉SignalR触发`OnConnected`事件实现的.如果你在调用start方法之前不注册任何event handlers,你可能不能invoke在Hub的方法,但是Hub的`OnConnected`方法将不会被调用并且没有客户端方法从服务器被invoked.

　**$.connection.hub 和 $.hubConnection()创建相同的对象**
正如你在例子中看到的,当你使用生成代理是,$.connection.hub指的是连接对象.这和你不使用代理时$.hubConnection创建的对象相同.生成代理的代码帮你执行如下的代码:
`// This line is at the end of the generated proxy code.`
`signalR.hub = $.hubConnection("/signalr", { useDefaultPath:false });`

当你使用生成代理,$.connection.hub可以替代不使用代理的连接对象,做任何事情.

## Asynchronous execution of the start method
`start`方法是异步执行的.返回[jQuery Deferred object](http://api.jquery.com/category/deferred-object/),这意味着你可以通过如pipe,done,fail添加回调函数.如果你想要在建立连接之后执行代码,如调用一个服务端的函数,可以把这段代码放在回调函数中或者用回调函数调用它.`.done`回调函数在建立完连接之后被执行,并且在服务端`OnConnected`event handler中所有代码执行完成之后.
如果前面的例子中把"Now connected"语句声明到调用start方法的下一行(不是在.done回调里),console.log这一行将会在建立连接之前被执行,结果就如下所示:
![](http://media-www-asp.azureedge.net/media/4275154/startasync.png)

## How to establish a cross-domain connection
通常情况下如果浏览器从http://contoso.com加载一个页面,SignalR连接在相同的domain中http://contoso.com/signalr.如果页面从http://contoso.com建立连接到http://fabrikam.com/signalr,这是一个跨域连接.出于安全的原因,跨域连接默认情况下是不可用的.
在SignalR 1.x中,跨域请求是通过一个单一的EnableCrossDomain标志控制的.这个标志也控制JSONP和CORS请求.为了更加灵活,所有的CORS支持已经从SignalR服务组件(如果检测到浏览器支持它,JavaScript客户端仍然正常使用CORS)中移除,并且新的OWIN中间件已经提供对这些场景的支持.
如果在客户端需求JSONP(支持旧的浏览器跨域请求),需要明确设置HubConfiguration对象中的EnableJSONP为true,如下所示.由于与CORS比不太安全,JSONP默认是不可用的.
**Adding Microsoft.Owin.Cors to your project:**在Package Manager Console中执行如下命令安装这个库:
`Install-Package Microsoft.Owin.Cors`
这个命令将会在项目中添加版本为2.1.0的包.

#### Calling UseCors
下面的代码演示了在SignalR 2中实现跨域连接.
　**Implementing cross-domain requests in SignalR 2**

下面代码演示了如何在项目中启用CORS或者JSONP.这段代码例子使用Map和RunSignalR替代MapSignalrR,从而使CORS中间件只运行于那些需要CORS支持的SignalR请求(而不是在中MapSignalR指定的路径的所有通信).Map还可以用于任何其他需要一个特定的URL前缀的中间件,而不是为整个应用程序的任何其他中间件.
```csharp
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;
namespace MyWebApplication
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Branch the pipeline here for requests that start with "/signalr"
            app.Map("/signalr", map =>
            {
                // Setup the CORS middleware to run before SignalR.
                // By default this will allow all origins. You can
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    // EnableJSONP = true
                };
                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}
```

> Notes:
>
>   * 不要设置jQuery.support.cors为true
> ![](http://media-www-asp.azureedge.net/media/4275148/jquerycors.png)
SignalR处理CORS的使用.设置jQuery.support.cors为true来禁用JSONP,它会导致SignalR承担浏览器支持CORS
>   
>   * 当你连接到一个localhost URL时,IE10不会认为它是一个跨域连接,所以应用会在本地IE10正常工作,即使你没有开启到服务端的跨域连接
>   * 关于使用IE9的跨域连接信息,请看[this StackOverflow thread](http://stackoverflow.com/questions/13573397/siganlr-ie9-cross-domain-request-dont-work)
>   * 关于Chromw的跨域连接信息,请看[this StackOverflow thread](http://stackoverflow.com/questions/15467373/signalr-1-0-1-cross-domain-request-cors-with-chrome)
>   * 例子中代码使用默认的"/signalr" URL连接到你的SignalR服务器.关于如何制定一个不同的base URL信息,请看[ASP.NET SignalR Hubs API Guide - Server - The /signalr URL](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-server#signalrurl)

## How to configure the connection
在你建立连接之前,你可以指定一个查询字符串参数或者制定传输方法.

#### How to specify query string parameters
如果在客户端连接时,您想将数据发送到服务器,你可以添加查询字符串参数到连接对象中.下面的示例显示了如何设置查询字符串参数的客户端代码:

　**在调用start方法前设置一个查询字符串值 (使用生成代理)**

`$.connection.hub.qs = { 'version' : '1.0' };`

　**在调用start方法前设置一个查询字符串值 (不适用生成代理)**

`var connection = $.hubConnection();`
`connection.qs = { 'version' : '1.0' };`

下面代码展示了在服务端如何读取一个查询字符串参数:
```csharp
public class ContosoChatHub : Hub
{
    public override Task OnConnected()
    {
        var version = Context.QueryString['version'];
        if (version != '1.0')
        {
            Clients.Caller.notifyWrongVersion();
        }
        return base.OnConnected();
    }
}
```

#### How to specify the transport method
作为连接处理的一部分,一个SignalR客户端通常与服务器协商来确定用于支持服务器和客户端最佳的传输方式.如果你早就知道使用哪种传输,当你调用start方法时,你可以制定传输方法绕过这个negotiation process(谈判进程).

　**客户端代码制定传输方法(使用生成代理)**

`$.connection.hub.start( { transport: 'longPolling' });`

　**客户端代码制定传输方法(不使用生成代理)**

`var connection = $.hubConnection();`
`connection.start({ transport: 'longPolling' });`

作为一种替代方法,你可以指定SignalR按顺序尝试多种传输方式.

　**指定自定义传输回退方案的客户端代码(使用生成代理)**

`$.connection.hub.start( { transport: ['webSockets', 'longPolling'] });`

　**指定自定义传输回退方案的客户端代码(不使用生成代理)**

`var connection = $.hubConnection();`
`connection.start({ transport: ['webSockets', 'longPolling'] });`

您可以使用下列值用于指定传输方法:

* "webSockets"
* "foreverFrame"
* "serverSentEvents"
* "longPolling"

下面的例子中展示了如何找出一个连接使用了哪种传输方法.
　**客户端代码显示连接使用的传输方法(使用生成代理)**

```JavaScript
$.connection.hub.start().done(function () {
    console.log("Connected, transport = " + $.connection.hub.transport.name);
});
```

　**客户端代码显示连接使用的传输方法(不使用生成代理)**

```
var connection = $.hubConnection();
connection.hub.start().done(function(){
    console.log("Connected, transport = "+ connection.transport.name);
});
```

关于如何在服务端检查使用了哪种传输方法信息,请看[ ASP.NET SignalR Hubs API Guide - Server - How to get information about the client from the Context property](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-server#contextproperty).
有关传输和回退的详细信息,请看[Introduction to SignalR - Transports and Fallbacks](http://www.asp.net/signalr/overview/signalr-20/getting-started-with-signalr-20/introduction-to-signalr#transports).

## How to get a proxy for a Hub class
每一个连接对象(你创建关于一个连接到SignalR服务的封装信息)包含一个或多个Hub classes.和一个Hub 类交流,使用一个自己创建的代理对象(如果你不使用生成代理)或者生成的代理对象.
在客户端代理的名字是Hub类名的驼峰命名形式.SignalR自动创建这个改变,所以JavaScript代码可以符合JavaScript约定.

　**服务端Hub类**

`public class ContosoChatHub : Hub`

　**得到Hub在客户端生成的代理**

`var myHubProxy = $.connection.contosoChatHub`

　**Create client proxy for the Hub class (without generated proxy)**

`var contosoChatHubProxy = connection.createHubProxy('contosoChatHub');`

如果用HubName属性修饰你的Hub类，就可以原样引用

　**服务端Hub类使用HubName属性修饰**

`[HubName("ContosoChatHub")]`
`public class ContosoChatHub : Hub`

　**得到Hub在客户端生成的代理**

`var myHubProxy = $.connection.ContosoChatHub`

　**Create client proxy for the Hub class (without generated proxy)**

`var contosoChatHubProxy = connection.createHubProxy('ContosoChatHub');`

## How to define methods on the client that the server can call
定义一个从服务端Hub中调用的方法,使用生成代理的client属性添加一个event handler到Hub代理中,或者调用on方法如果你没有使用生成代理.参数可以使复杂类型.

在调用start方法建立连接之前添加event handler.(如果你想要在调用start方法之后添加event handlers,看这篇文档的前面[How to establish a connection](#howtoestablishaconnection)和用于定义方法所示的句法而不使用生成的代理)

方法名称匹配是不区分大小写,如服务端的Clients.All.addContosoChatMessageToPage将会执行客户端的AddContosoChatMessageToPage, addContosoChatMessageToPage,addcontosochatmessagetopage中的任意方法.

　**Define method on client (with the generated proxy)**

```javascript
var contosoChatHubProxy = $.connection.contosoChatHub;
contosoChatHubProxy.client.addContosoChatMessageToPage = function (userName, message) {
    console.log(userName + ' ' + message);
};
$.connection.hub.start()
    .done(function(){ console.log('Now connected, connection ID=' + $.connection.hub.id); })
    .fail(function(){ console.log('Could not Connect!'); });
});
```

　**Alternate way(备用方式) to define method on client (with the generated proxy)**

```javascript
$.extend(contosoCharHubProxy.client, {
    addContosoChatMessageToPage: fucntion(userName,Message){
        console.log(userName + ' ' + message);
    };
});
```

　**Define method on client (without the generated proxy, or when adding after calling the start method)**

```javascript
var connection = $.hubConnection();
var contosoChatHubProxy = connection.createHubProxy('contosoChatHub');
contosoChatHubProxy.on('addContosoChatMessageToPage', function(userName, message) {
    console.log(userName + ' ' + message);
});
connection.start()
    .done(function(){ console.log('Now connected, connection ID=' + connection.id); })
    .fail(function(){ console.log('Could not connect'); });
```

　**Server code that calls the client method**

```csharp
public class ContosoChatHub : Hub
{
    public void NewContosoChatMessage(string name, string message)
    {
        Clients.All.addContosoChatMessageToPage(name, message);
    }
}
```

下面的实施例包括一个**复杂的对象**作为方法参数
　**Define method on client that takes a complex object (with the generated proxy)**

```javascript
var contosoChatHubProxy = $.connection.contosoChatHub;
contosoChatHubProxy.client.addMessageToPage = function (message) {
    console.log(message.UserName + ' ' + message.Message);
});
```

　**Define method on client that takes a complex object (without the generated proxy)**

```javascript
var connection = $.hubConnection();
var contosoChatHubProxy = connection.createHubProxy('contosoChatHub');
chatHubProxy.on('addMessageToPage', function (message) {
    console.log(message.UserName + ' ' + message.Message);
});
```

　**Server code that defines the complex object**

```csharp
public class ContosoChatMessage
{
    public string UserName { get; set; }
    public string Message { get; set; }
}
```

　**Server code that calls the client method using a complex object**

```csharp
public void SendMessage(string name, string message)
{
    Clients.All.addContosoChatMessageToPage(new ContosoChatMessage() { UserName = name, Message = message });
}
```

## How to call server methods from the client

要从客户端调用服务器的方法,使用生成的代理中的server属性或者不使用生成代理中的invoke方法.返回的值或者参数可以是复杂类型.
传入Hub方法名的驼峰命名版本.SignalR自动使这一变化使JavaScript代码能够符合JavaScript的约定.
下面的示例演示如何调用服务端没有返回值或者有返回值的方法.

　**服务端方法,没有HubMedtodName属性修饰**
```csharp
public class ContosoChatHub : Hub
{
    public void NewContosoChatMessage(ChatMessage message)
    {
        Clients.All.addContosoChatMessageToPage(message);
    }
}
```

　**服务端代码定义传入的复杂类型参数**
```csharp
public class ChatMessage {
    public string UserName{get;set;}
    public string Message{get;set;}
}
```

　**客户端代码调用服务端方法(使用生成代理)**
```javascript
contosoChatHubProxy.server.newContosoChatMessage({ UserName: userName, Message: message}).done(function () {
        console.log ('Invocation of NewContosoChatMessage succeeded');
    }).fail(function (error) {
        console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
    });
```

　**客户端代码调用服务端方法(不使用生成代理)**
```javascript
contosoCharHubProxy.invoke('newContosoChatMessage', {UserName: userName, Message: message}).done(function(){
    console.log ('Invocation of NewContosoChatMessage succeeded');
}).fail(function(error){
    console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
});
```

##### ***如果检测到Hub方法使用HubMethodName属性,使用定义的方法名称,而不是用驼峰版方法名.***

　**服务端方法**使用HubMethodName属性
```csharp
public class ContosoChatHub : Hub
{
    [HubMethodName("NewContosoChatMessage")]
    public void NewContosoChatMessage(string name, string message)
    {
        Clients.All.addContosoChatMessageToPage(name, message);
    }
}
```

　**客户端代码调用服务端方法(使用生成代理)**
```javascript
contosoChatHubProxy.server.NewContosoChatMessage({ UserName: userName, Message: message}).done(function () {
        console.log ('Invocation of NewContosoChatMessage succeeded');
    }).fail(function (error) {
        console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
    });
```

　**客户端代码调用服务端方法(不使用生成代理)**
```javascript
contosoCharHubProxy.invoke('NewContosoChatMessage', {UserName: userName, Message: message}).done(function(){
    console.log ('Invocation of NewContosoChatMessage succeeded');
}).fail(function(error){
    console.log('Invocation of NewContosoChatMessage failed. Error: ' + error);
});
```

*上面的代码展示了如何调用服务端没有返回值的方法.下面的例子展示如何调用服务端有返回值的方法.*

　**服务端有返回值的函数代码**
```csharp
public class StockTickerHub : Hub
{
    public IEnumerable<Sotck> GetAllStocks()
    {
        return _stockTicker.GetAllStocks();
    }
}
```
　**用于做返回值的Stock类**
```csharp
public class Stock
{
    public string Symbol { get; set; }
    public decimal Price { get; set; }
}
```
　**客户端调用服务端方法的代码(使用生成代理)**
```javascript
function init() {
    return stockTickerProxy.server.getAllStocks().done(function (stocks) {
        $.each(stocks, function () {
            var stock = this;
            console.log("Symbol=" + stock.Symbol + " Price=" + stock.Price);
        });
    }).fail(function (error) {
        console.log('Error: ' + error);
    });
}
```
　**客户端调用服务端方法的代码(不使用生成代理)**
```javascript
function init() {
    return stockTickerProxy.invoke('getAllStock').done(function(stocks){
        $.each(stocks, function(){
            var stock = this;
            console.log("Symbol=" + stock.Symbol + " Price=" + stock.Price);
        });
    }).fail(function(error){
        console.log('Error: '+error);
    });
}
```

## How to handle connection lifetime events
SignalR提供了如下你可以处理的连接生命周期事件:

* starting:通过该连接发送任何数据之前引发
* received:当连接上接收到任何数据引发,处理收到的数据.
* connectionSlow:当客户端检测到缓慢或经常丢失连接时引发(Raised when the client detects a slow or frequently dropping connection.)
* reconnection:当底层传输开始连接时引发
* reconnected:当底层传输已经连接时引发
* stateChanged:当连接状态改变时引发.提供旧的状态和新的状态(Connecting,Connected,Reconnecting or Disconnected).
* disconnected:当连接已断开时引发

例如,如果你期望当发生连接问题可能会有明显延迟时显示警告消息,处理connectionSlow事件.

**处理connectionSlow事件(使用生成的代理)**
```
$.connection.hub.connectionSlow(function () {
    console.log('We are currently experiencing difficulties with the connection.')
});
```

**处理connectionSlow事件(不使用生成的代理)**
```
var connection = $.hubConnection();
connection.connectionSlow(function(){
	console.log('We are currently experiencing difficulties with the connection.');
});
```

更多信息,请看[Understanding and Handling Connection Lifetime Events in SignalR.](http://www.asp.net/signalr/overview/signalr-20/hubs-api/handling-connection-lifetime-events)

## How to handle errors

SignalR js 客户端提供了一个可以为其添加handler的error事件.你也可以使用fail方法来处理从服务器方法调用产生的错误.

如果你不明确启用服务端的详细错误信息,出现错误后,SignalR返回的异常对象将包含最小的错误信息.例如,如果调用newContosoChatMessage失败,生成的错误消息,只会包含"There was an error invking Hub method 'contosoChatHub.newContosoChatMessage'.".出于安全原因,在生产环境下发送详细错误信息是不被推荐的,但是如果你想要明确详细错误用于故障排除,使用如下的服务端代码:

```
var hubConfiguration = new HubConfiguration();
hubConfiguration.EnableDetailedErrors = true;
app.MapSignalR(hubConfiguration);
```

*下面的例子演示了如何添加对错误事件的处理:*

**添加错误处理(使用生成的代理)**
```
$.connection.hub.error(function (error) {
    console.log('SignalR error: ' + error)
});
```

**添加错误处理(不使用生成的代理)**
```
var connection = $.hubConnection();
connection.error(functin(error){
	console.log('SignalR error: ' + error);
});
```

*下面的代码展示了如何在调用方法之后处理错误*
**方法调用后处理错误(使用生成的代理)**
```
contosoChatHubProxy.newContosoChatMessage(userName, message)
	.fail(function(error){
		console.log('newContosoChatMessage error: ' + error);
	});
```

**方法调用后处理错误(使用生成的代理)**
```
contosoChatHubProxy.invoke('newContosoChatMessage', userName, message)
	.fail(function(error){
		console.log('newContosoChatMessage error: ' + error);
	});
```

如果一个方法调用失败,错误处理事件也会被触发,所以你在error处理方法或者fail回调方法中的代码会被执行.

## How to enable client-side logging

在一个连接中启用客户端的日志记录,需要在调用start方法建立连接之前为连接对象设置`loggong`属性.

**启用logging(使用生成的代理)**
```
$.connection.hub.logging = true;
$.connection.hub.start();
```

**启用logging(不使用生成的代理)**
```
var connection = $.hubConnection();
connection.logging = true;
connection.start();
```

要查看日志,打开浏览器的开发者工具,跳到Consoel标签.对于一个教程，说明一步一步的说明和截图，显示如何做到这一点.请看[Server Broadcast whtd ASP.NET Signalr - Enable Logging.](http://www.asp.net/signalr/overview/signalr-20/getting-started-with-signalr-20/tutorial-server-broadcast-with-signalr-20#enablelogging)
