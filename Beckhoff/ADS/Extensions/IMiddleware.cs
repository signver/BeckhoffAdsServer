namespace Poh.PLC.Beckhoff.ADS.Extensions
{
    public delegate void RequestHandler();
    public interface IMiddleware
    {
        void Invoke(Protocol.AdsContext context, RequestHandler next);
    }
}
