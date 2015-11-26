# Server
ASP.NET 5和寄宿应用的网络环境完全没有关系.作为发布版本,ASP.NET 5支持IIS和IIS Express,WebListener和 Kestrel可以运行在各种平台的网络服务器.开发者和第三方软件提供商可以创建他们自己习惯的服务器供ASP.NET5应用寄宿.

##### In this artcle:
* Server and commands
* Supproted features by server
* IIS and IIS Express
* WebListener
* Kestrel
* Choosing a server
* Custom Servers
 
## Server and commands
ASP.NET5被设计为从承载它们在底层Web服务器分离的web应用程序.习惯上,ASP.NET应用程序只能在Windows上(除非通过mono)并且托管在内置的Web服务器的Windows上Internet信息服务器(IIS)或开发服务器,如IIS Express或早期开发的Web服务器.虽然IIS仍是推荐的方式来承载Windows上生产ASP.NET应用程序,ASP.NET的跨平台性使其能够在任意数量的不同的Web服务器,多个操作系统上进行托管.
  
ASP.NET5 附带三个不同的服务器:  

* Microsoft.AspNet.Server.IIS  
* Microsoft.AspNet.Server.WebListener (WebListener)
* Microsoft.AspNet.Server.Kestrel (Kestrel)  

ASP.Net 5不直接监听请求,而是依赖于HTTP服务器实现表面请求应用程序作为一组*feature interfaces*,组成一个HttpContext.IIS 和 WebListener都只支持Windows;Kerstrel用来跨平台运行.你可以在<code>project.json</code>中配置特殊的commands使应用程序寄宿在以上任何一个web服务器上.你甚至可以为应用程序指定一个应用程序入口点,并运行它作为一个可执行文件(使用<code>dnx run</code>)而不是托管在一个单独的进程.
使用VisualStudio 2015开发ASP.NET应用,默认寄宿在IIS/IIS Express.在<code>projcect.json</code>中"Microsoft.AspNet.Server.IIS"作为依赖项被包含,即使是空网站模板.VS支持多种配置,如IIS Express和<code>project.json</code>中定义的<code>commands</code>.你可以在项目属性中Debug下,管理这些配置设置.
> Note  
> IIS不支持commands,Visual Studio启动IIS Express并且把应用程序加载到你选择的配置对应的Web Server中.