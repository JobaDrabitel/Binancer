using System;
using System.Threading.Tasks;
using Bitfinex.Net;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Objects;
using Bittrex.Net.Clients;
using Kraken.Net.Clients;

namespace AllAPIsBot
{
    class AllAPIs
    {
        static async Task GetAPIsAsync(string[] args)
        {
            var bitfinexClient = new BitfinexSocketClient();
            var bittrexClient = new BittrexSocketClient();
            var krakenClient = new KrakenSocketClient();
            var btcSymbol = "btcusdt";
            var ethSymbol = "btcusdt";
            var btcDepth = await bittrexClient.SpotStreams.SubscribeToTickerUpdatesAsync(btcSymbol, data =>
            {
                Console.WriteLine($"Price of {btcSymbol}: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
            });

            var ethDepth = await bittrexClient.SpotStreams.SubscribeToTickerUpdatesAsync(ethSymbol, data =>
            {
                Console.WriteLine($"Price of {ethSymbol}: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
            });
            while (true) { }
        }
    }
}
