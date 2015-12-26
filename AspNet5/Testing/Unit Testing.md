# Unit Testing
ASP.NET 5 已经被设计成可测试的,因此为你的应用创建单元测试比以往更容易.这篇文章简要的介绍单元测试(并且和其他种类测试不同) 并且示范如何添加测试项目到你的解决方案里,然后同时使用cmd或者visual studio运行单元测试.
##### In this article  
* [Getting Started with Testing](#getting-started-with-testing)
* [Creating Test Projects](#creating-test-projects)
* [Running Tests](#running-tests)
* [Additional Resources](#additional-resources)
  
[Download sample from GitHub.](https://github.com/aspnet/docs/tree/1.0.0-beta8/aspnet/testing/unit-testing/sample)
## Getting Started with Testing<a id='#getting-started-with-testing'></a>  
一套自动化测试是最好的方法之一,确保软件应用程序作者想要做什么,对于软件应用的测试有很多种不同的方式,包括 [integration tests](), web tests, load tests, 和许多其他测试.在最底层的是单元测试,它测试独立的软件组件或者方法.单元测试应该只测试开发者控件中的代码,并且不应该测试基础设施,像 databases,file systems, 或者 network resources.单元测试也许使用 Test Driven Developemet(TDD测试驱动开发)书写,或者他们可以被添加到现有的代码中去确认他的正确性.不论那种情况, 他们应该小巧,命名规范并且运行快速,因为理想情况下,你将会运行数百个单元测试在将你的更改推送到共享代码库之前.  
> Note  
> 开发人员常常纠结于想出好的测试的类和方法名称.作为起点,ASP.NET产品团队遵循[这些约定]()

编写单元测试时,小心意外引入依赖基础设施.这些往往会使测试变慢和更脆弱,因此应该留给集成测试.你可以让你的应用程序代码遵从[Explicit Dependencies Principle(显示依赖原理)]()和使用依赖注入来请求你的依赖关系的框架来避免这些隐藏的依赖.你也可以在集成测试中保持你的单元测试在一个独立的项目中,并且确定你的单元测试项目没有引用或者依赖底层包.
## Creating Test Projects<a id='creating-test-projects'></a> 
测试项目仅仅是一个让test runner应用的类库并且是项目被测试(也被System Under Test or SUT引用).在你的项目中将测试项目放在一个单独的文件夹中是一个很好的习惯,并且作为DNX项目推荐的约定,项目结构如下:  
<pre>
global.json
PrimeWeb.sln
src/
  PrimeWeb/
    project.json
    Startup.cs
    Services/
      PrimeService.cs
test/
  PrimeWeb.UnitTests/
    project.json
    Services/
      PrimeService_IsPrimeShould.cs
</pre>
有一个文件夹或者目录命名为您测试的项目的名称是非常重要的(在PrimeWeb上面),因为文件系统会使用它去寻找你的项目.
### Configuring the Test project.json
测试项目的 <code>projcet.json</code> 应该添加所依赖使用的测试框架和SUT项目.例如,如果使用[xUnit test framework](http://xunit.github.io/),你将要配置如下依赖:  
<pre>
"dependencies": {
  "PrimeWeb": "1.0.0",
  "xunit": "2.1.0",
  "xunit.runner.dnx": "2.1.0-beta6-build191"
},
</pre>
对于其他为DNX发布的测试框架,我们将会在这里连接至他们.我们简单的使用作为众多不同测试框架中的一种---xUnit,插入到ASP.NET 和 DNX.
> Note  
> 确保版本号和你的project-to-project references匹配  
  
除了添加依赖关系,我们也希望能够使用DNX命令运行测试.这样做,添加以下部分命令到 <code>project.json</code> 中:  
<pre>
"commands": {
  "test": "xunit.runner.dnx"
}
</pre>
> Note  
> 在DNX中,学习更多关于[Using Commands](http://docs.asp.net/en/latest/dnx/commands.html)
  
## Running Tests<a id='running-tests'></a>
在你可以运行你的测试之前,你需要写一些.这个例子,我已经创建了一个简单的服务来检查数字是否质数.其中一个测试如下:  
<pre>
[Theory]
[InlineData(-1)]
[InlineData(0)]
[InlineData(1)]
public void ReturnFalseGivenValuesLessThan2(int value)
{
    var result = _primeService.IsPrime(value);

    Assert.False(result, String.Format("{0} should not be prime", value));
}
</pre>
这个测试将在三个独立的测试中,使用 <code>IsPrime</code> 方法检查1 0和1.每个测试会通过如果 <code>IsPrime</code> 返回false,否则测试不会通过。  
您可以从命令行运行测试或使用Visual Studio,看你更喜欢哪种方式.
### Visual Studio<p id='visual-studio'></p>
在Visual Studio上运行单元测试,首先打开Test Explorer tab,然后build解决方案确定发现所有可用测试.一旦你这么做了,您应该看到你所有的测试在Test Explorer Window.单击 Run All 运行测试并查看结果.

![](https://public.readthedocs.com/aspnet-aspnet/en/latest/_images/test-explorer1.png)  

如果你点击了 Run All的左上方按钮,Visual Studio将会在每次build完成之后运行测试,提供即时反馈当你工作在您的应用程序.
### Command Line
通过命令行来运行单元测试,定位到你的单元测试文件夹,然后运行如下命令:  
<pre>
dnx test
</pre>
你会看到相同的输出:
![](https://public.readthedocs.com/aspnet-aspnet/en/latest/_images/dnx-test.png)
##### dnx-watch
你可以使用 <code>dnx-watch</code> 命令时自动执行DNX命令当文件夹的内容改变时.这可以用作自动运行测试当文件在项目中被保存时.注意,这样将会同时检测SUT 项目和测试项目的改变,甚至从测试项目文件夹运行.
使用 <code>dnx-watch</code> ,简单地运行它并将其传递给原来的dnx命令参数.这样:  
<pre>
dnx-watch test
</pre>
当dnx-watch 运行时,你可以更新你的测试 和/或 您的应用程序,并在保存更改你应该可以看到测试又运行了一次.

![](https://public.readthedocs.com/aspnet-aspnet/en/latest/_images/dnx-watch.png)
一个自动化测试的主要好处是快速反馈测试规定,降低了引入和发现错误的时间.对于连续运行测试,无论是使用 <code>dnx-watch</code> 或者Visual Studio,当他们介绍的行为,打破有关程序应该如何运行的现有预期,开发人员可以立即发现.
> Tip   
> 查看[示例](https://github.com/aspnet/docs/tree/1.0.0-beta8/aspnet/testing/unit-testing/sample)的完整测试和服务行为.您可以运行web应用程序并导航到 <code>/ checkprime?5</code> 测试数据是否质数.您可以通过[Integration Testing](http://docs.asp.net/en/latest/testing/integration-testing.html)了解更多关于测试和重构这个checkprime网络行为.

## Additional Resources<a id='additional-resources'></a> 
* [*Testing*](https://public.readthedocs.com/aspnet-aspnet/en/latest/testing/integration-testing.html)
* [*Dependency Injection*](https://public.readthedocs.com/aspnet-aspnet/en/latest/fundamentals/dependency-injection.html)