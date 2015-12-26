# Request Features
### In this article:
* [Feature interfaces](#feature-interfaces)
* [Feature collections](#)
* [Middleware and request features](#)

## Feature interfaces<div id='feature-interfaces'></div>
ASP.NET 5 定义了许多由服务器使用的Http Feature Interfaces,来确定哪些是他们(服务器本身)支持的.一个服务器最基本功能是处理请求和返回响应的能力,所以定义如下的接口功能:
#### [IHttpRequestFeature]()
　　定义HTTP请求结构,包括protocol,path,query string,headers,和body.
#### [IHttpResponseFeature]()
　　定义HTTP响应结构,包括status code,headers,和响应body
#### [IHttAuthenticationFeature]()
　　Defines support for identifying users based on a ***ClaimsPrincipal*** and specifying an authentication handler.
#### [IHttpUpgradeFeature]()
　　Defines support for [HTTP Upgrades](), which allow the client to specify which additional protocols it would like to use if the server wishes to switch protocols.
#### [IHttpBufferingFeature]()
　　Defines methods for disabling buffering of requests and/or responses.
#### [IHttpConnectionFeature]()
　　Defines properties for local and remote addresses and ports.
#### [IHttpRequestLifetimeFeature]()
　　Defines support for aborting connections, or detecting if a request has been terminated prematurely, such as by a client disconnect.
#### [IHttpSendFileFeature]()
　　Defines a method for sending files asynchronously.
#### [IHttpWebSocketFeature]()
　　Defines an API for supporting web sockets.
#### [IRequestIdentifierFeature]()
　　Adds a property that can be implemented to uniquely identify requests.
#### [ISessionFeature]()
　　Defines <code>ISessionFactory</code> and <code>ISession</code> abstractions for supporting user sessions.
#### [ITlsConnectionFeature]()
　　Defines an API for retrieving client certificates.
#### [ITlsTokenBindingFeature]()
　　Defines methods for working with TLS token binding parameters.
> <code>ISessionFeature</code> 不是一个服务器功能,但是被 SessionMiddleware实现.

## Feature collections
HttpAbstractions存储库包含一个FeatureModel包.它的主要组成是常用于[服务器]()和服务器请求的[FeatureCollection]()类型,类似[*Middleware*](),确定哪些特性支持.[HttpContext]()类型被定义在***Microsoft.AspNet.Http.Abstractions***(不要和定义在***System.Web***中的<code>HttpContext</code>混淆),提供一个接口来获取和设置这些功能.因为是可变的特性集合,甚至在一个请求的上下文中,middleware可以用来修改集合并添加额外特性的支持.

## Middleware and request features
当服务器负责创建特性集合时,middleware可以添加到这个集合也可以从这个集合中移除特性.例如,[StaticFileMiddleware]()通过[StaticFileContext]()访问一个特性(<code>IHttpSendFileFeature</code>):

	public async Task SendAsync()
	{
		ApplyResponseHeaders(Constants.Status200ok);
		
		string physicalPath = _fileInfo.PhysicalPath;
		var sendFile = _context.GetFeature<IHttpSendFileFeature>();
		if (sendFile != null && !string.IsNullOrEmpty(physicalPath))
		{
			await sendFile.SendFileAsync(physicalPath, 0, _length, _context.RequestAborted);
			return;
		}
		
		Stream readStream = _fileInfo.CreateReadStream();
		try{
			await SteamCopyOperation.CopyToAsync(readStream, _response.Body, _length, _context.RequestAborted);
		}
		finally{
			readStream.Dispose();
		}
	}
上面的代码中,<code>StaticFileContext</code>类的<code>SendAsync</code>方法访问服务器<code>IHttpSendFileFeature</code>特性的实现(调用<code>HttpContext</code>中的<code>GetFeature</code>).如果特征存在,它被用来从它的物理路径发送所请求的静态文件.否则一个慢得多的解决方法用于发送文件.(掌握的<code>IHttpSendFileFeature</code>允许操作系统打开文件，并进行直接内核模式拷贝到网卡)
> Note   
> 从中间件或应用程序中使用上述特征检测显示模式。如果支持该功能,调用<code>GetFeature</code>将会返回一个借口如果特性被支持,否则返回null
  
另外,middleware可以添加到由服务器建立的特性集合中,通过调用<code>SetFeature<></code>.存在的特性甚至可以被middleware替换,允许middleware增大服务器的功能.加入到集合中的特性在之后的请求管线中会立即提供给其他middleware或应用程序本身使用.  
  
[WebSocketMiddleware]()遵循这种方法,首先检查服务器是否支持升级(<code>IHttpUpgradeFeature</code>),然后添加一个new <code>IHttpWebSocketFeature</code>到特性集合,如果它不存在. 另外,如果配置成 替换现有的实现(via <code>_options.ReplaceFeature</code>),它将重写所有现有的实现.
<pre>
public Task Invoke(HttpContext context)
{
	// Detect if an opaque upgrade is available. If so, add a websocket upgrade.
    var upgradeFeature = context.GetFeature<IHttpUpgradeFeature>();
    if (upgradeFeature != null)
    {
            if (_options.ReplaceFeature || context.GetFeature<IHttpWebSocketFeature>() == null)
            {
                    context.SetFeature<IHttpWebSocketFeature>(new UpgradeHandshake(context,
                            upgradeFeature, _options));
            }
    }

    return _next(context);
}
</pre>
  
通过结合自定义服务器的实现和特定的中间件增强功能，精确的特性集的应用需要可以构造。这使得无需在服务器的改变要添加缺少的功能，并确保只有特征的最小量被暴露，因此限制了攻击表面积和提高性能

##Summary
