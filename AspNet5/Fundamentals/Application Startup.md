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
