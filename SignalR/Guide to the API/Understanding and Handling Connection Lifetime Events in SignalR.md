# 理解和处理SignalR中连接生命周期事件
<p style='background-color:#EFEFEF'>
这篇文章描述了SignalR中你可以处理的connection,reconnection,disconnection事件,和超时时间及保持连接的设置.
<br />
本文假定您已经有了SignalR和连接周期事件有一定的了解.关于SignalR的介绍
[Introduction to SignalR](http://www.asp.net/signalr/overview/signalr-20/getting-started-with-signalr-20/introduction-to-signalr).有关连接生命周期事件列表,如下:
<ul>
<li>[How to handle connection lifetime events in the Hub class](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-server#connectionlifetime)</i>
<li>[How to handle connection lifetime events in JavaScript clients](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-javascript-client#connectionlifetime)</i>
<li>[How to handle connection lifetime events in .NET clients](http://www.asp.net/signalr/overview/signalr-20/hubs-api/hubs-api-guide-net-client#connectionlifetime)</i>
</ul>
</p>

## 连接生命周期术语和场景
在SignalR Hub中可以直接执行的OnReconnected事件处理在OnConnected之后执行,而不是客户端OnDisconnected之后.
