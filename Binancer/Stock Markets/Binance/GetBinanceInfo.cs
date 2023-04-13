using Binance.Net.Clients;
using Binance.Net.Objects;

namespace Binancer
{
    internal class GetBinanceInfo
    {
        public static BinanceApiCredentials GetApiCredentials()
        {
            var apiCredentials = new BinanceApiCredentials("<API key>", "<secret key>");

            return apiCredentials;
        }

        public static BinanceClient GetBinanceClient(BinanceApiCredentials apiCredentials)
        {
            var client = new BinanceClient(new BinanceClientOptions()
            {
                ApiCredentials = apiCredentials,
            });

            return client;
        }
    }
}