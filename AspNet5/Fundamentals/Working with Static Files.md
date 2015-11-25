# Working with Static Files
静态文件包括HTML,CSS,images,javascript文件,可以直接被客户端访问,在这篇文章里,我们将讨论以下议题，因为它们涉及到ASP.NET5和静态文件.
##### In this article:
* Serving static files
* Enabling directory browsing
* Serving default files
* Using the UseFileServer method
* Working with content types
* IIS considerations
* Best practices

## Serving static files
默认,静态文件存储在项目的*webroot*里.这个webroot的位置定义在项目的<code>project.json</code>文件里,默认配置为:
<pre>"webroot":"wwwroot"</pre>
静态文件可以存储在webroot下任何文件夹并且使用相对路径来访问根.例如,当你使用vs创建一个默认的web应用程序项目时,将会有几个文件夹被创建到webroot中- css,images和js.为了能直接访问images子目录下的图片,URL可能如下:  
　　http://&lt;yourApp>/images/&lt;imageFileName>  
为了能让静态文件被服务器处理,你必须配置***Middleware***去添加静态文件到管线中.这可以通过调用<code>Startup</code>类中<code>Configure</code>方法下的<code>UseStaticFiles</code>扩展方法完成,如下:  
<pre>
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
  ...
  // Add static files to the request pipeline.
  app.UseStaticFiles();
  ...
</pre>
现在,如果你有一个存放静态文件的文件夹,但是它没有放在webroot下.例如:

* wwwroot
	* css
	* images
	* ...    
* MyStaticFiles
	* test.png  

为了用户可以访问test.png,你可以配置静态文件中间件如下:  
<pre>
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
  ...
  // Add MyStaticFiles static files to the request pipeline.
  app.UseStaticFiles();
  app.UseStaticFiles(new StaticFileOptions()
  {
      FileProvider = new PhysicalFileProvider(@"D:\Source\WebApplication1\src\WebApplication1\MyStaticFiles"),
      RequestPath = new PathString("/StaticFiles")
  });
  ...
</pre>

如果直接输入 http://&lt;yourApp>/StaticFiles/test.png,就能直接获取test.png

## Enabling directory browsing
