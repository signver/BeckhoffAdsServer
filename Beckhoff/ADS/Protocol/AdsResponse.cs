using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poh.PLC.Beckhoff.ADS.Protocol
{
    public class AdsResponse
    {
        public AdsResponse(AdsCommand command)
        {
            Command = command;
            Payload = null;
            Error = Error.GeneralError;
        }

        public AdsCommand Command
        {
            get;
            private set;
        }

        public Error Error
        {
            get;
            set;
        }

        public byte[] Payload
        {
            get;
            set;
        }
    }
}
