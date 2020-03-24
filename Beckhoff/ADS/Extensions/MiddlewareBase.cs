using Poh.PLC.Beckhoff.ADS.Protocol;

namespace Poh.PLC.Beckhoff.ADS.Extensions
{
    public abstract class MiddlewareBase : IMiddleware
    {
        // Executes while the handler chain progresses to the next middleware
        // Return true to prevent further propagation on the request handler chain
        protected abstract bool Capture(AdsContext context);
        // Executes while the handler chain returns to the previous middleware
        protected abstract void Bubble(AdsContext context);
        public void Invoke(AdsContext context, RequestHandler next)
        {
            if (!Capture(context))
            {
                next();
            }
            Bubble(context);
        }
    }
}
