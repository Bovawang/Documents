## Application Startup

Asp.Net 5为你的应用程序如何处理请求提供了完整控制.`Startup`类是应用程序的入口点,设置配置项和连接应用服务将会用到.开发人员在`Startup`类中配置一个用于处理对应用程序所有请求的request pipeline.

### The Startup class
在Asp.Net5里,`Startup`类作为应用的入口点,任何applications必须具备.可能有特定环境的`startup`类和方法(see [Working with Multiple Environments](http://docs.asp.net/en/latest/fundamentals/environments.html)).Asp.Net搜索名为Startup的主要assembly(在任何命名空间下),你可以使用*Hosting:Application*配置键指定搜索一个不同的assembly.这个类是否定义成public无关紧要;如果它符合命名约定Asp.Net仍然会加载它.如果有多个`Stratup`类,不会触发异常,Asp.Net将会根据命名空间选取一个(首先匹配项目的根命名空间,除此之外将命名空间按照字母排序,并使用第一个能找到的类).

`Startup`类可以选择在它的构造函数中接受[dependency injection](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html)提供的依赖性.通常,应用程序将要被配置的方式是定义在它的Startup类的构造方法中(see [Configuration](http://docs.asp.net/en/latest/fundamentals/configuration.html)).Startup类必须定义一个`Configure`方法,并且也可以选择定义一个`ConfigureServices`方法,当应用程序启动的时候会被调用.

###The Configure method
`Configure`方法用于指定Asp.Net应用如何响应individual HTTP请求.简单的说,你可以配置所有请求接受相同的响应.然而,多数real-world应用程序需要更多的功能.更加复杂的pipeline集合配置可以封装到[middleware](http://docs.asp.net/en/latest/fundamentals/middleware.html)中并且使用`IApplicationBuilder`的扩展方法添加.
你的`Configure`方法必须接受一个`IApplicationBuilder`参数.额外的服务,像`IHostingEnvironment`和`ILoggerFactory`也通常会被指定,这种情况下,如果这些服务可用将会被服务器[injected](http://docs.asp.net/en/latest/fundamentals/dependency-injection.html).在下面的例子中,你可以看到几种扩展方法,用于配置pipeline与支持[BrowserLink](http://www.asp.net/visual-studio/overview/2013/using-browser-link),error pages, static files, Asp.Net MVC, 和Identity.
```
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    loggerFactory.AddConsole(Configuration.GetSection("Logging"));
    loggerFactory.AddDebug();

    if (env.IsDevelopment())
    {
        app.UseBrowserLink();
        app.UseDeveloperExceptionPage();
        app.UseDatabaseErrorPage();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

    app.UseStaticFiles();

    app.UseIdentity();

    // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

    app.UseMvc(routes =>
    {
        routes.MapRoute(
            name: "default",
            template: "{controller=Home}/{action=Index}/{id?}");
    });
}
```
你可以通过源码检查每一个扩展方法在做什么.例如,`UseMvc`这个扩展方法是定义在`BuilderExtensions`中,可以在[GigHub](https://github.com/aspnet/Mvc/blob/6.0.0-beta5/src/Microsoft.AspNet.Mvc/BuilderExtensions.cs)上查看.它主要的作用是确保MVC作为一个服务被添加并且为Asp.Net MVC应用正确建立routing规则.

你可以学习所有关于middleware并且使用`IApplicationBuilder`去定义你自己的请求pipeline在[Middleware](http://docs.asp.net/en/latest/fundamentals/middleware.html)章节.

### The ConfigureServices method
你的Startup类可以包括一个ConfigureServices方法,此方法用于配置应用程序所要使用的服务.ConfigureServices方法在Startup类中使用public修饰,使用IServiceCollection接口作为参数,可以选择返回IServiceProvider类型.ConfigureServices方法在Configure方法之前调用是非常重要的,因为一些功能如:ASP.NET MVC需要在被接入到请求pipeline之前添加某些服务.

正如Configure,它推荐那些需要在ConfigureServices内部大量设置的功能,都作为IServiceCollection的扩展方法封装起来.在下面的例子中你可以看到一些Add[Something]扩展方法,它们用来配置应用来使用:Entity Framework,Identity和MVC:
```
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddEntityFramework()
        .AddSqlServer()
        .AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

    services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    services.AddMvc();

    // Add application services.
    services.AddTransient<IEmailSender, AuthMessageSender>();
    services.AddTransient<ISmsSender, AuthMessageSender>();
}
```

在你的应用通过dependency injection向服务容器中添加服务,使它们可用.正如Startup类能够指定依赖它的方法需要作为参数,而不是硬编码到一个特定的实现,所以你的middleware MVC控制器和其他类也可以这样做.

ConfigureServices方法也是添加配置选项类的地方,例如,你在应用中想让Appsetting可用.更多关于配置选项可以参考[Configuration](http://docs.asp.net/en/latest/fundamentals/configuration.html).

### Services Available in Startup
ASP.NET 5 在应用启动期间提供一些应用服务和对象.你可以在Startup类的构造函数或COnfigure方法或ConfigureServices方法里包括适当的接口作为参数来启用某个特定的服务.这些服务在Startup类包含的一下方法中都是可用的.提供的框架服务和对象包括:
**IApplicationBuilder**

    * 用于构建应用请求pipeline.只在Startup类中的Configure方法中可用.了解更多[Request Features](http://docs.asp.net/en/latest/fundamentals/request-features.html)

**IApplicationEnvironment**

    * 提供访问应用的属性,如ApplicationName,ApplicationVersion,ApplicationBasePath
    * 在Startup的构造方法和Configure方法中可用

    
**IHostingEnvironment**

    * 提供当前EnvironmentName,WebRootPath和web root file provider
    * 在Startup的构造方法和Configure方法中可用

**ILoggerFactory**

    * 用于创建日志
    * 在Startup的构造方法和Configure方法中可用 了解更多[Logging](http://docs.asp.net/en/latest/fundamentals/logging.html)

**IServiceCollection**

    * 容器中已经配置的服务集合
    * ConfigureServices方法中可用,并且使用该方法来配置可用于应用程序的服务

按照被调用的顺序来看Startup类的每个方法,以下的服务可以作为参数请求:
Startup Constructor - `IApplicationEnvironment` - `IHostingEnvironment` - `ILoggerFactory`

ConfigureServices - `IServiceCollection`

Configure - `IApplicationBuilder` - `IApplicationEnvironment` - `IHostingEnvironment` - `ILoggerFactory`

>Although ILoggerFactory is available in the constructor, it is typically configured in the Configure method. Learn more about Logging.

