using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TwinCAT.Ads;
using TwinCAT.Ads.Server;
namespace Poh.PLC.Beckhoff.ADS.Internal
{
    public delegate void RequestHandler(Protocol.AdsContext context);
    // Extension to the base ADS server class
    // Adds events and context handling
    class GeneralizedServer : TcAdsServer
    {
        public event RequestHandler OnClientReadRequest;
        public event RequestHandler OnClientWriteRequest;
        public event RequestHandler OnClientReadWriteRequest;

        private Dictionary<uint, Task> _pendingRequests;
        private CancellationTokenSource _cancellationSource; 

        public GeneralizedServer(ushort port, string name) : base(port, name)
        {
            _cancellationSource = new CancellationTokenSource();
            _pendingRequests = new Dictionary<uint, Task>();
        }

        public new void Dispose()
        {
            _cancellationSource.Cancel();
            base.Dispose();
        }

        public override void AdsWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, uint cbLength, byte[] data)
        {
            if (OnClientWriteRequest != null)
            {
                _pendingRequests.Add(
                    invokeId,
                    Task.Factory.StartNew(() => {
                        Protocol.AdsContext context = new Protocol.AdsContext(rAddr.ToString(), Protocol.AdsCommand.Write, invokeId, indexGroup, indexOffset, 0, data);
                        OnClientWriteRequest(context);
                        ServiceRequest(context);
                    }, _cancellationSource.Token)
                );
            }
        }
        public override void AdsReadInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, uint cbLength)
        {
            if (OnClientReadRequest != null)
            {
                _pendingRequests.Add(
                    invokeId,
                    Task.Factory.StartNew(() => {
                        Protocol.AdsContext context = new Protocol.AdsContext(rAddr.ToString(), Protocol.AdsCommand.Read, invokeId, indexGroup, indexOffset, cbLength, null);
                        OnClientReadRequest(context);
                        ServiceRequest(context);
                    }, _cancellationSource.Token)
                );
            }
        }
        public override void AdsReadWriteInd(AmsAddress rAddr, uint invokeId, uint indexGroup, uint indexOffset, uint cbReadLength, uint cbWriteLength, byte[] data)
        {
            if (OnClientReadWriteRequest != null)
            {
                _pendingRequests.Add(
                    invokeId,
                    Task.Factory.StartNew(() => {
                        Protocol.AdsContext context = new Protocol.AdsContext(rAddr.ToString(), Protocol.AdsCommand.ReadWrite, invokeId, indexGroup, indexOffset, cbReadLength, data);
                        OnClientReadWriteRequest(context);
                        ServiceRequest(context);
                    }, _cancellationSource.Token)
                );
            }
        }

        private void ServiceRequest(Protocol.AdsContext context)
        {
            Protocol.AdsResponse response = context.Response;
            switch (response.Command)
            {
                case Protocol.AdsCommand.Read:
                    base.AdsReadRes(AmsAddress.Parse(context.Route), context.InvokeID, MapError(response.Error), response.Payload == null ? 0 : (uint)response.Payload.Length, response.Payload);
                    break;
                case Protocol.AdsCommand.Write:
                    base.AdsWriteRes(AmsAddress.Parse(context.Route), context.InvokeID, MapError(response.Error));
                    break;
                case Protocol.AdsCommand.ReadWrite:
                    base.AdsReadWriteRes(AmsAddress.Parse(context.Route), context.InvokeID, MapError(response.Error), response.Payload == null ? 0 : (uint)response.Payload.Length, response.Payload);
                    break;
                default:
                    break;
            }
            if (_pendingRequests.ContainsKey(context.InvokeID))
            {
                _pendingRequests.Remove(context.InvokeID);
            }
        }

        private AdsErrorCode MapError(Protocol.Error error)
        {
            switch (error)
            {
                case Protocol.Error.NoError:
                    return AdsErrorCode.NoError;
                case Protocol.Error.ApiError:
                    return AdsErrorCode.ClientError;
                default:
                    return AdsErrorCode.InternalError;
            }
        }
    }
}
