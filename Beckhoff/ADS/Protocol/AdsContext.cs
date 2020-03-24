using System;
namespace Poh.PLC.Beckhoff.ADS.Protocol
{
    public class AdsContext
    {
        public AdsContext(string address, AdsCommand command, uint invokeid, uint group, uint offset, uint length, byte[] payload)
        {
            Route = !string.IsNullOrEmpty(address) ? address : throw new ArgumentNullException();
            InvokeID = invokeid;
            Request = new AdsRequest(command, group, offset, length, payload);
            Response = new AdsResponse(command);
            CreationTime = DateTime.Now.Ticks;
        }
        public long CreationTime
        {
            get;
            private set;
        }
        public string Route
        {
            get;
            private set;
        }
        public uint InvokeID
        {
            get;
            private set;
        }
        public AdsRequest Request { get; private set; }
        public AdsResponse Response { get; private set; }
    }
}
