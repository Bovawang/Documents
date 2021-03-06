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
在SignalR Hub中可以直接执行的OnReconnected处理程序在OnConnected之后执行,而不是客户端OnDisconnected之后.The reason you can have a reconnection without a disconnection is that there are several ways in which the word "connection" is used in SignalR.

#### SignalR连接,传输连接,物理连接
下面会指出三者的不同:

* **SignalR connection** :指的是客户端和**服务器URL**之间的逻辑关系,由SignalR API维持并唯一通过connection ID识别.有关此关系的数据被SignalR维护,并用于建立传输连接. The relationship ends and SignalR disposes of the data when the client calls the Stop method or a timeout limit is reached while SignalR is attempting to re-establish a lost transport connection.
* **Transport connection** :指的是客户端和**服务器**之间的逻辑关系,由四个传输API WebSockets,server-sent events,forever frame,long polling中的一个维持.SianalR使用传输API创建传输连接,并且传输API取决于物理网络连接是否存在来创建传输连接的.当SignalR终止或者当传输API检测到物理连接断开时transport connection结束.
* **Physical connection** :指的是物理网络连接--wires,wireless signals,routes等等--便于客户端计算机和服务器计算机之间的通信. 想要建立transport connection,物理连接必须存在. 并且想要建立SignalR连接 transport connection也必须被建立. 然而, 断开物理连接通常不会终止transport connection或者SignalR连接,稍后会解释原因.

下面的途中,SignalR层中Hubs API和PersistentConnection API 代表SignalR连接,Transports层代表transport connection,物理连接用服务端和客户端中间的线表示.
![](..\img\intro_architecture.png)

当你在SignalR客户端调用Start方法时,你要提供带有建立到服务端物理连接的所有信息的SignalR客户端代码.客户端代码使用这些信息建立一个HTTP请求,并且使用四个传输方法中的一个建立物理连接.如果传输连接失败或者服务端未响应,该SignalR连接不马上消失，因为客户端仍然需要这些信息来自动重新建立一个新的传输连接到相同的SignalR URL.在这个场景下,没有来自用户的应用程序的干预涉及, 当SignalR客户端代码建立一个新的传输连接, 它不启动新的SignalR的连接.SignalR连接的保持实际上被反射到connection ID中,这个ID在你调用Start方法时创建,不会改变.

Hub上的OnReconnected处理程序在传输连接已经丢失后自动建立时执行.
OnDisconnected处理程序在SignalR连接结束后执行.一个SignalR连接可以通过以下任意方式结束:
* 如果客户调用Stop方法,一条停止消息被发送到服务器,并且客户端和服务器立即结束SignalR连接
* 客户端和服务器之间的连接丢失后, 客户端尝试重新连接, 服务器等待客户端重新连接.如果重新连接尝试不成功和断开连接的超时期限结束, 客户端和服务器立即结束SignalR连接. 客户端将停止尝试重新连接, 服务器释放SignalR连接的标识
* 如果客户端没有调用Stop方法而停止运行, 服务端等待客户端重新连接, 然后在断开连接超时后中断SignalR连接
* 如果服务端停止运行, 客户端尝试重新连接(重新创建transport connection), 然后在断开连接超时后中断SignalR连接

当没有连接问题出现时, 并且用户应用调用Stop方法结束SignalR连接, SignalR连接和传输连接开始和结束大约在同一时间.以下部分描述更详细的其他场景:

#### Transport断开连接场景
物理连接可能会减缓或连通性受到干扰. 根据长度等因素的干扰, Transport connection可能会丢失. SignalR然后试图重新建立传输连接. 有时传输连接API检测到中断并且放弃传输连接, SignalR会马上发现连接丢失了. 在其他场景里, 无论是传输连接API还是SignalR都会立即意识到连接已经丢失. 对于除了long polling以外所有的传输方法, SignalR客户机使用一个名为keepalive检查损失的函数连接传输API是否丢失连接. 关于长轮询连接的信息, 请看本篇文章的[Timeout and keepalive settings](http://www.asp.net/signalr/overview/guide-to-the-api/handling-connection-lifetime-events#timeoutkeepalive)话题.

当一个连接是不活跃的,服务器定期的向客户端发送一个keepalive包.截至日期写这篇文章时,默认频率是每10秒.  通过侦听这些包,客户端指出是否有连接问题.如果预期没有收到keepalive报文, 过一段时间后客户端认为连接出现问题,如连接缓慢或中断.如果较长时间后keepalive仍没有收到, 客户端假设连接已经被中断, 并且开始试着重新连接.

下面的图表说明了客户端和服务器事件在一个典型的场景当物理连接有问题,不是立刻认识到由传输API.图适用于下列情形:

* 传输方法是WebSockets,forever frame,或者server-sent events
* 物理网络连接有不同中断期间
* 传输API没有察觉中断,所以SignalR依赖保持活动来检测他们
 
![](..\img\transportdisconnections.png)

如果client进入重连模式并且不能在超时限制内建立传输连接,服务端会终止SignalR连接.当这种情况发生,服务端会执行Hub的OnDisconnected方法并且queues up a disconnect message to send to the client in case the client manages to connect later.. 如果客户端之后不重连, 它接收到disconnect消息并且调用Stop方法. 这种情况下, 当客户端重连时OnReconneted方法不会被执行, 并且客户端调用Strop方法后OnDisconnected也不会被执行. 下面图解释了这个场景:

![](..\img\transportdisruptionstimeout.png)


***SignalR连接生命周期事件可能被延长, 如果客户端发生以下事件***:

* ConnectionSlow client event
自从上个消息或keepalive ping被接收起, 已经超过keepalive timeout period的预设比例,将会延长. 默认keepalive timeout warining period是keepalive timeout的2/3. keepalive timeout 是20秒, 所以warning 发生在大约13秒左右.
通常, 服务端发送keepalive pings 每10一次, 并且客户端每2秒检查一次keepalive pings(keepalive timeout value和keepalive timeout warning value之间的差额的三分之一).
如果传输API察觉到一个disconnection, SignalR会在keepalive timeout warning period到期之前收到通知. 这种情况, ConnectionSlow事件将会被触发, 并且SignalR会直接调用Reconnecting事件.

* Reconnecting client event
a). 当传输API检测到连接丢失时被触发
b). 上次收到消息后keepalive timeout period(超时连接期限)已过,或收到keepalive ping
SignalR客户端代码开始试着重新连接.传输连接丢失后如果你希望你的应用程序采取一些行动, 你可以处理这个事件. 当前默认超时连接期限是20秒.
当SignalR处于重连模式时,如果你的客户端代码尝试调用Hub方法, SignalR会视图发送命令. 大多数情况下, 这样的尝试将失败, 但在某些情况下, 他们可能会成功. 对于server-sent events, forever frame,和long polling传输, SignalR使用两个通信信道, 一个客户端用来发送消息, 另一个用来接收消息. 用于接收的信道是永久开放的，只有在物理连接中断时是关闭的. 用于发送的通道仍然可用, 因此, 如果恢复物理连接, a method call from client to server might be successful before the receive channel is re-established.直到SignalR重新打开用于接收的通道后才会返回值.

* Reconnected client event
当传输连接重新建立时触发. 在Hub中OnReconnected事件处理执行.

* Closed client event(disconnected event in JavaScript)
当SignalR客户端代码在失去连接后试图重新连接而disconnect timeout period超期时触发. 默认的disconnect timeout是30秒.(当调用Stop方法导致连接中断时也会触发这个客户端事件)

传输API没有检测到传输连接中断,and don't delay the reception of keepalive pings from the server for longer than the keepalive timeout warning period might not cause any connection lifetime events to be raised.

有些网络环境中故意关闭空闲连接,and another function of the keepalive packets is to help prevent this by letting these networks know that a SignalR connection is in use. 在极端情况下的违约频率keepalive ping可能不足以防止关闭连接. 在这种情况下您可以配置更频繁的发送keepalive ping. 更多信息,请看文章后面的[Timeout and keepalive settings](http://www.asp.net/signalr/overview/guide-to-the-api/handling-connection-lifetime-events#timeoutkeepalive).

> **Important**:The sequence of events described here is not guaranteed. SignalR makes every attempt to raise connection lifetime events in a predictable manner according to this scheme, but there are many variations of network events and many ways in which underlying communications frameworks such as transport APIs handle them. For example, the Reconnected event might not be raised when the client reconnects, or the OnConnected handler on the server might run when the attempt to establish a connection is unsuccessful. This topic describes only the effects that would normally be produced by certain typical circumstances.


#### Client 断开连接场景
http://www.asp.net/signalr/overview/guide-to-the-api/handling-connection-lifetime-events
