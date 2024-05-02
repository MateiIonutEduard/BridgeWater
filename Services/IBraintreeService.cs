using Braintree;

namespace BridgeWater.Services
{
    /* Braintree interface used to do implement virtual payments. */
    public interface IBraintreeService
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
