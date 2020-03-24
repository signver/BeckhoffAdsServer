
namespace Poh.PLC.Beckhoff.ADS.Protocol
{
    public class AdsRequest
    {
        public AdsRequest(AdsCommand command, uint group, uint offset, uint length, byte[] payload)
        {
            Command = command;
            IndexGroup = group;
            IndexOffset = offset;
            ReadLength = length;
            Payload = payload;
        }

        public AdsCommand Command
        {
            get;
            private set;
        }

        public uint InvokeID
        {
            get;
            private set;
        }

        public uint ReadLength
        {
            get;
            private set;
        }

        public uint IndexGroup
        {
            get;
            private set;
        }

        public uint IndexOffset
        {
            get;
            private set;
        }

        public byte[] Payload
        {
            get;
            private set;
        }

        public uint WriteLength
        {
            get => Payload == null ? 0 : (uint)Payload.Length;
        }
    }
}
