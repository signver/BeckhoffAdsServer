namespace Poh.PLC.Beckhoff.ADS.Protocol
{
    public enum AdsCommand
    {
        Invalid,
        Read,
        Write,
        ReadWrite
    }
    public enum Error
    {
        NoError,
        ApiError,
        GeneralError,
    }
}
