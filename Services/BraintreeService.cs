using Braintree;

namespace BridgeWater.Services
{
    public class BraintreeService : IBraintreeService
    {
        readonly IConfiguration config;

        public BraintreeService(IConfiguration config)
        { this.config = config; }

        public IBraintreeGateway CreateGateway()
        {
            var newGateway = new BraintreeGateway()
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = config.GetValue<string>("BraintreeGateway:MerchantId"),
                PublicKey = config.GetValue<string>("BraintreeGateway:PublicKey"),
                PrivateKey = config.GetValue<string>("BraintreeGateway:PrivateKey")
            };

            return newGateway;
        }

        public IBraintreeGateway GetGateway() 
        { return CreateGateway(); }
    }
}
