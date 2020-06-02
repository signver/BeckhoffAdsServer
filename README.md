# Overview
This is a wrapper of the Beckhoff `TcAdsServer` base class, allowing the usage of middlewares to extend the server functionalities. 


# General usage

`namespace Poh.PLC.Beckhoff.ADS`

 
The `Server` object is the go-to object to be instantiated in the main program. It converts the requests into the `AdsContext` object. The `AdsContext` is passed on to the middleware to be handled accordingly.


<pre><code>
Server _server = new Server(_port, _name);
// Additional stuffs
// Add middlewares
// ...
_server.Start(); 
</code></pre>


# Middleware

`namespace Poh.PLC.Beckhoff.ADS.Extensions`

Extend the abstract MiddlewareBase class or implement the IMiddleware interface.


## MiddlewareBase

When implementing the MiddlewareBase class, override the `bool Capture(AdsContext)` and `void Bubble(AdsContext)` method. Middlewares can be chained together, and the chain propagation can be stopped by returning `true` in the `Capture` method.

Sample middleware implementation

<pre><code>
class MyMiddleware : MiddlewareBase {
&emsp;public override void Bubble(AdsContext context) { 
&emsp;&emsp;//Do stuff
&emsp;&emsp;//Otherwise
&emsp;&emsp;return;
&emsp;}
&emsp;public override bool Capture(AdsContext context) { 
&emsp;&emsp;if (context.Request.Command == AdsCommand.Read && context.Request.IndexGroup == 0x1000) {
&emsp;&emsp;&emsp;if (context.Request.IndexOffset == 0x0001 && context.Request.Length == 0x0003) {
&emsp;&emsp;&emsp;&emsp;context.Reponse.Error = Error.ApiError;
&emsp;&emsp;&emsp;} 
&emsp;&emsp;&emsp;else {
&emsp;&emsp;&emsp;&emsp;context.Reponse.Error = Error.NoError;
&emsp;&emsp;&emsp;&emsp;context.Response.Payload = new byte[] {0,1,2};
&emsp;&emsp;&emsp;}
&emsp;&emsp;&emsp;return true; //To prevent further propagation down the middleware chain
&emsp;&emsp;}
&emsp;&emsp;return false: 
&emsp;}
}
</code></pre>

Sample middleware usage

<pre><code>
Server _server = new Server(_port, _name);
_server.Use(new MyMiddleware);
// ...
_server.Start();
</code></pre>


## Middleware Chaining

    //...
    _server.Use(MiddlewareA);
    _server.Use(MiddlewareB);
    _server.Use(MiddlewareC);
    //...

The `AdsContext` will be processed MiddlewareA.Capture &#8594; MiddlewareB.Capture &#8594; MiddlewareC.Capture &#8594; MiddlewareC.Bubble &#8594; MiddlewareB.Bubble &#8594; MiddlewareA.Bubble  


