using System;
using System.Collections.Generic;
namespace Poh.PLC.Beckhoff.ADS
{
    public class Server : IDisposable
    {
        private Internal.GeneralizedServer _server;
        private List<Extensions.IMiddleware> _middleware;
        public Server(ushort port, string name) 
        {
            _middleware = new List<Extensions.IMiddleware>();
            _server = new Internal.GeneralizedServer(port, name);
            _server.OnClientReadRequest += _server_OnClientReadRequest;
            _server.OnClientWriteRequest += _server_OnClientWriteRequest;
            _server.OnClientReadWriteRequest += _server_OnClientReadWriteRequest;
        }

        public void Start()
        {
            _server.Connect();
        }
        public void Use(Extensions.IMiddleware middleware)
        {
            _middleware.Add(middleware);
        }

        private void HandleClientRequest(Protocol.AdsContext context)
        {
            var iterator = _middleware.GetEnumerator();
            Extensions.RequestHandler next = null;
            next = () =>
            {
                if (iterator.MoveNext())
                {
                    iterator.Current.Invoke(context, next);
                }
            };
            next();
        }

        private void _server_OnClientReadWriteRequest(Protocol.AdsContext context)
        {
            HandleClientRequest(context);
        }

        private void _server_OnClientWriteRequest(Protocol.AdsContext context)
        {
            HandleClientRequest(context);
        }

        private void _server_OnClientReadRequest(Protocol.AdsContext context)
        {
            HandleClientRequest(context);
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
