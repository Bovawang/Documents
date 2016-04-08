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
![](img\angular-solution-explorer.png)
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



#### Key Components
AngularJS包含许多主要组件,如 *directives*,*templates*,*repeaters*,*modules*,*controllers*等等.让我们测试下,这些组件如何组合使用.


##### 指令(Directives)
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
![](img/simple-directive.png)
![](http://docs.asp.net/en/latest/_images/simple-directive.png)

其他AngularJS原始指令包括:
`ng-controller`
　　决定了JavaScript的控制器绑定到哪个视图
`ng-model`
　　确定到一个HTML元素的属性的值绑定模型
`ng-init`
　　用于初始化在表达的形式的应用程序数据为当前范围
`ng-if`
　　根据所提供的表达的true或false,移除或重新创建DOM中的给定的HTML元素
`ng-repeat`
　　循环输出HTML数据
`ng-show`
　　显示或隐藏根据所提供的表达式给定的HTML元素

对于AngularJS支持的所有原始指令的完整列表, 请参考[ directive documentation section on the AngularJS documentation website](https://docs.angularjs.org/api/ng/directive)


##### 数据绑定(Data Binding)
AngularJS使用`ng-bind`指令或者一个数据绑定表达式语法如`{{expression}}`提供[data binding](https://docs.angularjs.org/guide/databinding)来支持 out-of-the-box. AngularJS支持双向数据绑定，其中从模型数据在任何时候都保持在同步视图模板. 该视图的任何更改会自动反映在模型中. 同样, 任何模型的更改都会在视图上自动反映.

创建一个带有视图的HTML文件或者一个控制器方法, 命名为:Data并ing. 包含以下的视图:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Databinding</h1>

    <div ng-init="firstName='John'; lastName='Doe';">
        <strong>First name:</strong> {{firstName}} <br />
        <strong>Last name:</strong> <span ng-bind="lastName" />
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

请注意, 你可以使用指令或数据绑定(ng-bind)显示模型值. 结果如下所示:

![](img\simple-databinding.png)
![](http://docs.asp.net/en/latest/_images/simple-databinding.png)


##### 模板(Templates)
AngularJS中的[模板(Templaes)](https://docs.angularjs.org/guide/templates)就是一些带有AngularJS指令和部件HTML页面. 一个AngularJS模板是directives, expressions, filters, 和 controls混合HTML形成的视图.

添加另外一个视图用于演示模板:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Templates</h1>

    <div ng-init="personName='John Doe'">
        <input ng-model="personName" /> {{personName}}
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

这个模板带有AngularJS的指令,如:`ng-app`,`ng-init`,`ng-model`和数据绑定表达式语法来绑定`personame`属性. 在浏览器中运行, 结果如下:

![](img\simple-templates-1.png)
![](http://docs.asp.net/en/latest/_images/simple-templates-1.png)

如果在输入框内改变name, 你将会看到在label中的name自动更新,如下所示:

![](img\simple-templates-2.png)
![](http://docs.asp.net/en/latest/_images/simple-templates-2.png)


##### 表达式(Expressions)
AngularJS中的[表达式(Expressions)](https://docs.angularjs.org/guide/expression)类似JavaScript代码片段, 写在`{{ expression }}` 语法中. 这些表达式的数据和`ng-bind`指令使用同样的方式绑定到HTML. AngularJS表达式和常规的JavaScript表达式之间的主要区别是:AngularJS表达式针对AngularJS的`$scope`对象进行评估.
下面例子中的AngularJS表达式绑定`personName`和一段简单的JavaScript计算表达式:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Expressions</h1>

    <div ng-init="personName='John Doe'">
        Person's name is: {{personName}} <br />
        Simple JavaScript calculation of 1 + 2: {{1+2}}
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

结果如下:

![](img\simple-expressions.png)
![](http://docs.asp.net/en/latest/_images/simple-expressions.png)


##### 中继器(Repeaters)
AngularJS中的重复是通过调用原始指令`ng-repeat`实现的. `ng-repeat`指令在视图上根据重复数据数组的长度重复输出给定的HTML元素. AngularJS的中继器可以重复字符串数组或对象数组. 下面是重复字符串数组的样例:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Repeaters</h1>

    <div ng-init="names=['John Doe', 'Mary Jane', 'Bob Parker']">
        <ul>
            <li ng-repeat="name in names">
                {{name}}
            </li>
        </ul>
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

重复指令输出一系列的列表项无序列表，你可以在这个截图所示的开发工具看:

![](img\repeater.png)
![](http://docs.asp.net/en/latest/_images/repeater.png)

下面是重复输出对象数组. `ng-init`指令建立一个`names`数组, 每一个数组对象包含firstName和lastName. `ng-repeat`使用`name in names`输出每一个数组元素:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Repeaters2</h1>

    <div ng-init="names=[
        {firstName:'John', lastName:'Doe'},
        {firstName:'Mary', lastName:'Jane'},
        {firstName:'Bob', lastName:'Parker'}]">
        <ul>
            <li ng-repeat="name in names">
                {{name.firstName + ' ' + name.lastName}}
            </li>
        </ul>
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>

```

和上面输出的结果一样.

Angular提供了一些额外的指令, 提供基于循环执行的行为( can help provide behavior based on where the loop is in its execution).


* `$index`
在`ng-repeat`循环中使用`$index`来确定当前循环到哪一个位置.

* `$even` 和 `$odd`
使用`$even`确定在循环中,当前位置的index是否是偶数行. 同样,使用`$odd`确定当前位置的index是否是奇数行.

* `$first` 和 `$last`
使用`$first`确定在循环中当前index是否是第一行. 同样, 使用`$last`确定当前位置的index是否是最后一行.

下面演示了`$index`, `$even`, `$odd`, `$first`, 和 `$last`在实际中的应用:

```html
@{
    Layout = "";
}
<html>
<body ng-app>
    <h1>Repeaters2</h1>

    <div ng-init="names=[
        {firstName:'John', lastName:'Doe'},
        {firstName:'Mary', lastName:'Jane'},
        {firstName:'Bob', lastName:'Parker'}]">
        <ul>
            <li ng-repeat="name in names">
                {{name.firstName + ' ' + name.lastName}} at index {{$index}}
                <span ng-show="{{$first}}">, the first position</span>
                <span ng-show="{{$last}}">, the last position</span>
                <span ng-show="{{$odd}}">,which is odd-numbered.</span>
                <span ng-show="{{$even}}">,which is even-numbered.</span>
            </li>
        </ul>
    </div>

    <script src="~/lib/angular/angular.js"></script>
</body>
</html>
```

结果为:

![](img\repeaters2.png)
![](http://docs.asp.net/en/latest/_images/repeaters2.png)


##### $scope
`$.scope`是一个JavaScript对象,用于连接视图(模板)和控制器(在下面说明). AngularJS中的视图模板只知道连接到控制器中的$scope对象的值.

> In the MVVM world, the $scope object in AngularJS is often defined as the ViewModel. The AngularJS team refers to the $scope object as the Data-Model. [Learn more about Scopes in AngularJS](https://docs.angularjs.org/guide/scope).
 

下面演示了在分离的JavaScript文件`scope.js`中,如何在`$scope`上设置属性:

```javascript
var personApp = angular.module('personApp', []);
personApp.controller('personController', ['$scope', function ($scope) {
    $scope.name = 'Mary Jane';
}]);
```
观察在line 2中传递到控制器的`$scope`参数. 视图知道的这个对象. 在 line 3 ,我们设置了一个值为"Mary Jane"的属性"name".

当未由视图发现一个特定属性, 会发生什么?下面的视图定义"name"和"age"两个属性:
```html
@{
    Layout = "";
}
<html>
<body ng-app="personApp">
    <h1>Scope</h1>

    <div ng-controller="personController">
        <strong>Name:</strong> {{name}} <br />
        <strong>Missing Property (age):</strong> {{age}}
    </div>

    <script src="~/lib/angular/angular.js"></script>
    <script src="~/app/scope.js"></script>
</body>
</html>
```
注意9行我们让AngularJS使用表达式语法显示"name"属性. 10行指定"age", 一个不存在的属性. 运行结果显示name被设置为"Marry Jane",age没有值. 消失的属性被忽略.

![](img\scope.png)
![](http://docs.asp.net/en/latest/_images/scope.png)


##### 模块(Modules)
AngularJs中的模块是控制器,服务,指令等等的集合. 在AngularJS中调用`angular.module()`函数要使用create, register和retrieve模块. 所有模块,包括AngularJS团队附带的和第三方库, 都应该使用angular.module()函数进行注册.
下面的代码片段演示了如何在AngularJS中创建一个新的模块. 第一个参数是模块的名字. 第二个参数定义了所以来的其他模块. 在文章的后面, 我们将会演示如何调用`angular.module()`方法,并传入依赖项.

`var personApp = angular.module('persoanApp', []);`

使用ng-app指令来表示AngularJS模块在页面上. 填入模块的名称来使用此模块. 本例中是personApp:

`<body ng-app="personApp"></body>`


##### 控制器(Controllers)
在AngularJS中控制器是进入你的代码的入口点. `<modlue name>.controller()`的方法调用,用于在AngularJS中创建和注册控制器. `ng-contorller`指令用于代表一个AngularJS控制器在HTML页面上. 在angular控制器的作用是设置数据模型($scope)的状态和行为. 控制器不应该被用来直接操作DOM.

下面的代码片段演示了注册一个新的控制器. 代码中第二行定义的`personApp`变量引用了一个AngularJS模块.

```javascript
// module
var personApp = angular.module('personApp', []);

// controller
personApp.controller('personController', function ($scope) {
    $scope.firstName = "Mary";
    $scope.lastName = "Jane"
});
```
视图使用`ng-contorller`指令指定控制器名称:

```html
@{
    Layout = "";
}
<html>
<body ng-app="personApp">
    <h1>Controllers</h1>

    <div ng-controller="personController">
        <strong>First Name:</strong> {{firstName}} <br />
        <strong>Last Name:</strong> {{lastName}}
    </div>

    <script src="~/lib/angular/angular.js"></script>
    <script src="~/app/controllers.js"></script>
</body>
</html>
```

页面展示了"Mary"和"Jane"分别对应附加到$scope上的属性`firstName`和`lastName`:

![](img\controllers.png)
![](http://docs.asp.net/en/latest/_images/controllers.png)


##### 服务(Services)
在AngularJS中,服务通常用于该被抽象远到其可在整个角应用的生命周期中使用的文件共享代码(Services in AngularJS are commonly used for shared code that is abstracted away into a file which can be used throughout the lifetime of an Angular application). 服务是延迟实例化的, 意味着除非依赖于该服务的组件被使用, 否则不会有一个服务的一个实例. 在AngularJS应用中, 工厂是使用服务的一个例子. 工厂的创建要调用`myApp.factory()`函数, 其中myApp是一个模块.

下面的例子演示了如何在AngularJS中使用工厂:
```javascript
personApp.factory('personFactory', function () {
    function getName() {
        return "Mary Jane";
    }

    var service = {
        getName: getName
    };

    return service;
});
```
在控制器里调用工厂, 将`personFactory`作为一个参数传入到`controller`函数中:
```javascript
personApp.controller('personController', function($scope,personFactory) {
  $scope.name = personFactory.getName();
});
```

###### 使用服务跟一个REST端点通信(Using services to talk to a REST endpoint)
下面是一个使用服务AngularJS与一个ASP.NET的Web5端点API交互的终端到终端的的例子. 这个例子从Web API获取数据, 并在一个视图模板显示数据. 让我们先从第一个观点:
```html
@{
    Layout = "";
}
<html>
<body ng-app="PersonsApp">
    <h1>People</h1>

    <div ng-controller="personController">
        <ul>
            <li ng-repeat="person in people">
                <h2>{{person.FirstName}} {{person.LastName}}</h2>
            </li>
        </ul>
    </div>

    <script src="~/lib/angular/angular.js"></script>
    <script src="~/app/personApp.js"></script>
    <script src="~/app/personFactory.js"></script>
    <script src="~/app/personController.js"></script>
</body>
</html>
```
在这个视图中,我们调用一个Angular模块`PersonApp`,一个控制器`personController`. 使用`ng-repeat`遍历persons集合. 在17到19行引用三个自定义的javascript文件.

`personApp.js`文件用于注册`PersonApp`模块;语法和之前的例子相同. 我们使用的是`angular.module`函数来创建将要使用的模块的新实例.
```javascript
(function () {
    'use strict';
    var app = angular.module('PersonsApp', []);
})();
```
在`personFactory.js`文件中调用模块的方法`factory`创建一个工厂. 12行展示了Angular内建的`$http`服务从web服务获取人的信息.
```javascript
(function () {
    'use strict';

    var serviceId = 'personFactory';

    angular.module('PersonsApp').factory(serviceId,
        ['$http', personFactory]);

    function personFactory($http) {

        function getPeople() {
            return $http.get('/api/people');
        }

        var service = {
            getPeople: getPeople
        };

        return service;
    }
})();
```

在`personController.js`中,我们调用模块中的`controller`方法去创建一个控制器.`$scope`对象的`people`属性用于存放从personFactory返回的数据(13行);
```javascript
(function () {
    'use strict';

    var controllerId = 'personController';

    angular.module('PersonsApp').controller(controllerId,
        ['$scope', 'personFactory', personController]);

    function personController($scope, personFactory) {
        $scope.people = [];

        personFactory.getPeople().success(function (data) {
            $scope.people = data;
        }).error(function (error) {
            // log errors
        });
    }
})();
```

让我们快速浏览下ASP.NET5 Web API. `Person`是一个POCO(plain Old CLR Object), 带有Id, Firstname 和 LastName属性:
```csharp
namespace AngularSample.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
```

`Person`控制器返回一个Json格式的`Person`对象:
```csharp
using AngularSample.Models;
using Microsoft.AspNet.Mvc;
using System.Collections.Generic;

namespace AngularSample.Controllers.Api
{
    public class PersonController : Controller
    {
        [Route("/api/people")]
        public JsonResult GetPeople()
        {
            var people = new List<Person>()
            {
                new Person { Id = 1, FirstName = "John", LastName = "Doe" },
                new Person { Id = 1, FirstName = "Mary", LastName = "Jane" },
                new Person { Id = 1, FirstName = "Bob", LastName = "Parker" }
            };

            return Json(people);
        }
    }
}
```

运行上面程序:
![](img\rest-bound.png)
![](http://docs.asp.net/en/latest/_images/rest-bound.png)

You can [view the application’s structure on GitHub.](https://github.com/aspnet/Docs/tree/master/aspnet/client-side/angular/sample)

> Note
> For more on structuring AngularJS applications, see [John Papa’s Angular Style Guide](https://github.com/johnpapa/angular-styleguide)

> Note
> To create AngularJS module, controller, factory, directive and view files easily, be sure to check out Sayed Hashimi’s [SideWaffle template pack for Visual Studio](http://sidewaffle.com/). Sayed Hashimi is a Senior Program Manager on the Visual Studio Web Team at Microsoft and SideWaffle templates are considered the gold standard. At the time of this writing, SideWaffle is available for Visual Studio 2012, 2013, and 2015.

##### 路由和多视图(Routing and Multiple Views)
