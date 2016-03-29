## Using Angular for Single Page Applications(SPAs)

本文介绍,如何使用AngularJS构建一个SPA-style的ASP.NET应用.
[View this article's samples on GitHub](https://github.com/aspnet/Docs/tree/master/aspnet/client-side/angular/sample)

### What is AngularJS?
AngularJS是谷歌现代化的JavaScript框架,通常用于单页面应用程序(SPAs),开源遵循MIT license,你可以follow AngularJS的开发进度在[GitHub repository](https://github.com/angular/angular.js).

AngularJS与Jquery不同,不是DOM操作库.但是它使用叫jQLite的jQuery子集合.AngularJS主要是基于HTML声明属性，你可以添加到您的HTML标签.你可以在浏览器中练习AngularJS通过[Code School website](http://campus.codeschool.com/courses/shaping-up-with-angular-js/intro)网站.

1.5.x版本为稳定版本和Angular的团队正在开发V2.0,相对于之前版本将会是大部分的重写.本文重点介绍1.X和在那里Angularjs与2.0标题的一些注意事项

### Getting Started
开始在ASP.NET中应用AngularJS前,你必须要在项目中引入或者使用CDN.
#### Installation
有几种将AngularJS添加到项目的方法,如果你使用VS2015新建立一个ASP.NET5项目,可以使用内建的Bower添加:打开bower.json,添加依赖属性:
```
{
  "name": "ASP.NET",
  "private": true,
  "dependencies": {
    "bootstrap": "3.3.5",
    "jquery": "2.1.4",
    "jquery-validation": "1.14.0",
    "jquery-validation-unobtrusive": "3.2.4",
    "angular": "1.4.8",
    "angular-route": "1.4.8"
  }
}
```

保存上面的文件,AngularJS将会安装到你项目的`wwwroot/lib`文件夹.另外,它还会在`Dependencies/Bower`文件夹下列出.如下:

![](http://docs.asp.net/en/latest/_images/angular-solution-explorer.png)

接下来,在HTML页面或者_layout.cshtml的body中添加`<script>`引用,如下:
```
<environment names="Development">
    <script src="~/lib/jquery/dist/jquery.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.js"></script>
    <script src="~/lib/angular/angular.js"></script>
</environment>
```
建议在生产环境下使用CDNs.你可以引用下面的一个:
```
 <environment names="Staging,Production">
    <script src="//ajax.aspnetcdn.com/ajax/jquery/jquery-2.1.4.min.js"
        asp-fallback-src="~/lib/jquery/dist/jquery.min.js"
        asp-fallback-test="window.jQuery">
    </script>
    <script src="//ajax.aspnetcdn.com/ajax/bootstrap/3.3.5/bootstrap.min.js"
        asp-fallback-src="~/lib/bootstrap/dist/js/bootstrap.min.js"
        asp-fallback-test="window.jQuery && window.jQuery.fn && window.jQuery.fn.modal">
    </script>
    <script src="https://ajax.googleapis.com/ajax/libs/angularjs/1.4.8/angular.min.js"
        asp-fallback-src="~/lib/angular/angular.min.js"
        asp-fallback-test="window.angular">
    </script>
    <script src="~/js/site.min.js" asp-append-version="true"></script>
</environment>
```
如果你已经引用了angular.js文件,可以准备在web pages里使用了.

### Key Components
AngularJS包含许多主要组件,如 *directives*,*templates*,*repeaters*,*modules*,*controllers*等等.让我们测试下,这些组件如何组合使用.

#### Directives
AngularJS使用[directives](https://docs.angularjs.org/guide/directive)自定义属性和元素来扩展HTML.AngularJS的directives是通过`data-ng-*`或者`ng-*`前缀(`ng`是angular的缩写)定义的.共有两种类型的AngularJS directives:

1. **Primitive Directives**:这个类型是Angular团队预定义的,是AngularJS框架的一部分
2. **Custom Directives**:可以自己定义的自定义类型

所有AngularJS应用程序中使用的原始指令之一是ng-app指令,作为AngularJS应用它是必须的.这个指令可以添加到`<body>`标签或者body的子元素中.假设你在使用ASP.NET项目,你可以在wwwroot文件夹添加一个HTML,或者添加一个新的控制器并为其添加一个视图.我们在HomeController.cs文件中添加一个Directives方法.关联的视图如下:
```
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Directives</h1>
    {{2+2}}
    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

为了保持这些例子相互独立,我们不适用共享布局文件(_layout.cshtml).你可以看到我们使用`ng-app`指令修饰body标签,标识这个page是一个AngularJS应用.`{{2+2}}`是Angular数据绑定表达式,一会你将会学习它.下面是运行之后的界面:
![](http://docs.asp.net/en/latest/_images/simple-directive.png)

其他AngularJS原始指令包括:
`ng-controller`
　　
`ng-model`
　　
`ng-init`
　　
`ng-if`
　　
`ng-repeat`
　　
`ng-show`
　　