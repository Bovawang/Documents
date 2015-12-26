# DNX Overview

## 什么是.NET Execution Environment  
.NET运行环境(DNX)是一个软件开发工具(SDK)和运行环境,具备需要在Windows,Mac,Linux平台上编译和运行.NET应用程序的任何东西.它提供了一个宿主进程,CLR寄宿逻辑 和 管理入口点发现.DNX用于构建跨平台的ASP.NET Web应用程序，同时也可以运行其他类型的.NET应用,例如跨平台的控制台输出应用.

## 为什么构建DNX?
**跨平台 .NET开发**DNX提供了多平台(Windows,Mac,Linux)多框架(.NET Framework,.NET Core,Mono)下统一的开发和运行环境.通过DNX你可以在一个平台下开发应用同时在多个平台运行,只要具备与平台相对应版本的DNX.
**Bulid for .NET Core** DNX极大的简化了使用.NET Core开发跨平台应用.他负责托管CLR,处理依赖关系和你的应用程序.你可以简单的使用轻量级的JSON格式文件(project.json)创建项目和解决按方案,编译你的项目或者发布他们.
**Package ecosystem** 包经理已经完全改变了现代软件开发的面貌,DNX很容易创建和使用包.DNX提供安装,创建和管理NuGet包的工具.DNX项目通过交叉编译多个目标框架简化构建NuGet包,能够直接输出NuGet包.你可以从你的项目中直接饮用NuGet包来传递依赖关系处理.你也可以构建和安装开发工具软件包为你的项目.
**Open source friendly** DNX便于使用开源项目.通过DNX项目你可以方便的替换现有依赖它的源码,并且让DNX在运行时编译它到内存中.然后通过调试源码和修改它而不需要应用的剩余部分.

## Projects
一个DNX项目就是一个存在project.json的文件夹.项目的名称是文件夹的名字.你使用DNX项目构建NuGet包.*project.json*文件定义你的package metadata,项目袭来和你想为那个框架构建应用:
```
{
    "version":"1.0.0-*",
    "description": "ClassLibrary1 Class Library",
    "authors": [ "daroth" ],
    "tags": [ "" ],
    "projectUrl": "",
    "licenseUrl": "",
    
    "frameworks":{
        "net451":{},
        "dotnet5.4":{
            "dependencies":{
                "Microsoft.CSharp": "4.0.1-beta-23516",
                "System.Collections": "4.0.11-beta-23516",
                "System.Linq": "4.0.1-beta-23516",
                "System.Runtime": "4.0.21-beta-23516",
                "System.Threading": "4.0.11-beta-23516"
            }
        }
    }
}
```  
文件夹中的所有文件默认为项目的一部分,除非在project.json文件明确排除.
你也可以定义可执行的commands作为项目的一部分(see [Commands](https://docs.asp.net/en/latest/dnx/overview.html#dnx-concept-commands)).
在"frameworks"属性下制定你想要构建的frameworks.DNX会交叉编译每一个制定的frameworks并且在构建NuGet包是建立相应的lib文件夹.
你可以使用.NET Development Utility(DNU)去编译,打包或者发布DNX项目.编译一个项目生成二进制结果.打包项目生成一个可以被上传到package feed(例如[http://nuget.org](http://nuget.org))的NuGet包然后使用.发布会收集所有必需的运行时构件(所需的DNX和包)到一个文件夹中,以便它可以作为应用程序部署.
了解更多关于working DNX projects细节,请看[Working with DNX Projects](https://docs.asp.net/en/latest/dnx/projects.html).

## Dependencies
在DNX中Dependencies包括一个名字和一个版本号.版本号要遵循[Semantic Versioning](http://semver.org/).通常依赖关系需要一个已安装的NuGet包或者另一个DNX项目.项目引用解析当前项目使用对等文件夹或者项目指定使用*global.json*文件在解决方案层面:
```
{
    "projects":["src","test"],
    "sdk""{
        "version":"1.0.0-rc1-final"
    }
}
```  
*global.json*文件通常定义需要构建项目最低的DNX版本("sdk" version).
Dependencies传递您只需要指定顶级依赖关系.DNX将会为你使用已安装的NuGet包处理分析全部的依赖曲线.项目引用在运行时通过在内存中构建引用的项目来解决.这意味着你有充分的灵活性以包二进制文件或源代码部署DNX应用程序.

## Packages and feeds
为了解决包依赖问题Packages and Feeds必须被首先安装.以可以使用DNU来安装一个新的包到已经存在的项目中或者restore项目中所有依赖的包.下面的命令会下载安装所有列在project.json文件中的包:
```
dnu restore
```  
包是恢复使用配置的package feeds.你可以使用(NuGet configuration files(NuGet.config)](http://docs.nuget.org/consume/nuget-config-file)源配置可用的pagkage feeds.

## Commands
一个命令就是一个.NET中带有指定参数可执行入口点的命名.你可以定义commands在你的*project.json*文件中:
```
"commands":{
    "web":"Microsoft.AspNet.Server.Kestrel",
    "ef":"EntityFramework.Commands"
},
```
然后可以使用DNX去执行项目中定义的commands,例如:
```
dnx web
```
命令可以构建和分布式NuGet包。然后,您可以使用DNU在一台机器上安装globally命令:
```
dnu commands install MyCommand
```
有关使用和创建命令的更多信息,请看[Using Commands](https://docs.asp.net/en/latest/dnx/commands.html).
## Application Host
DNX应用主机通常是第一个成功的入口点调用DNX和负责处理依赖项解析,解析project.json,提供额外的服务和调用应用程序的入口点.
作为一种选择,你可以直接使用DNX调用应用程序的入口点.这需要你应用程序完全被编译并且所有的依赖都位于一个单独的目录.在没有DNX应用宿主的环境下使用DNX是不常见的.
DNX应用宿主为应用提供了一组服务,通过依赖注入(for example, IServiceProvider, IApplicationEnvironment and ILoggerFactory).应用程序宿主服务可以在类的构造函数注入你的主要入口点或作为你的主要入口点额外的方法参数.
## Compile Modules
编译模块是一个扩展点,让你参与DNX编译过程.通过实现[IComplieModule](https://github.com/aspnet/dnx/blob/dev/src/Microsoft.Dnx.Compilation.CSharp.Abstractions/ICompileModule.cs)接口来实现一个Compile Module.并且将你的编译模块放到一个complier/preprocess或者complier/postprocess在你的项目中.
## DNX Version Manager
你可以安装多版本的DNX和flavors在你的机器上.安装和管理不同版本的DNX和flavors你要使用.NET Version Manager(DNVM).DNVM可以列出你机器上不同版本的DNX和flavors,安装一个新的版本或者切换到activeDNX.
请看[Getting Started](https://docs.asp.net/en/latest/getting-started/index.html)关于DNVM的介绍