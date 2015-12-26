# Choosing the Right .NET For You on the Server
ASP.NET 5是基于 *[.NET Execution Environment(DNX)](https://public.readthedocs.com/aspnet-aspnet/en/latest/dnx/overview.html)*,它支持跨平台,可以运行在Windows,Mac和Linux.当你选择一个使用一个DNX时,也可以选择不同的运行时：.NET Framework(CLR), *[.NET Core(CoreCLR)](https://public.readthedocs.com/aspnet-aspnet/en/latest/conceptual-overview/dotnetcore.html)* or [Mono](http://mono-project.com/).要选择哪个?让我们看看他们的有点和缺点.

## .NET Framework
.NET Framework是最熟悉的,而且是三个中最稳定的.是windows下稳定包含全部功能的框架. .NET Framework生态系统是容易建立的,发布到现在已超过十年.NET Framework为今天的生产做好准备，提供了兼容性的最高水平为现有的应用程序和库.
.NET Framework只能运行在Windows上,同时也是一个具备大量API,较慢发布周期的单片组件.虽然.NET Framework可以[根据reference来参考](http://referencesource.microsoft.com/),但它不是一个活跃的开源项目.
## .NET Core
.NET Core 5是一个模块化的运行时和库实现,包括.NET Framework的子集..NET Core同时被Windows,Mac,Linux支持..NET Core包含一个叫"CoreFX"的库集合,和一个精致、被优化的运行时："CoreCLR"..NET Core是开源的,因此你可以在[GitHub](https://github.com/dotnet)上更随项目进展并且完善它.
CoreCLR运行时（Microsoft.CoreCLR）和CoreFX库通[过NuGet](https://www.nuget.org/)分布.
## Mono
## Summary
