using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitfinex.Net.Clients;
using Bitfinex.Net.Enums;
using Bitfinex.Net.Objects;
using CryptoExchange.Net.Authentication;

namespace CryptoBot
{
    class BitfinexBot
    {
        static ApiCredentials apiCredentials = GetApiCredentials();
        static BitfinexSocketClient? BitfinexSocketClient = GetBitfinexSocketClient(apiCredentials);
        static readonly Coin[] BitfinexCoins = new Coin[]
     {
           new Coin { Symbol = "tBTCUSD", BuyPrice = 29500m, SellPrice = 30000m },
           new Coin { Symbol = "tETHUSD", BuyPrice = 1800m, SellPrice = 19000m },
           new Coin { Symbol = "tTRXUSD", BuyPrice = 0.06530m, SellPrice = 0.06535m }
     };

        static ApiCredentials GetApiCredentials()
        {
            var apiCredentials = new ApiCredentials("<API key>", "<secret key>");
            return apiCredentials;
        }
        public static async Task BitfinexCheckAsync()
        {
            var apiCredentials = GetApiCredentials();
            for (int i = 0; i < BitfinexCoins.Length; i++)
                await GetCryptoDataAsync(BitfinexCoins[i].Symbol);
        }
        public static async Task GetCryptoDataAsync(string symbol)
        {
            var lastBid = 0m;
            var lastAsk = 0m;
            if (BitfinexSocketClient == null) { return; }
            var depth = await BitfinexSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(symbol, async data =>
            {
                if (data.Data.BestBidPrice != lastBid)
                {
                    Console.WriteLine($"Price of {symbol} in Bitfinex: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    //await BuyAsync(symbol, myQuantity, data.Data.BestBidPrice);
                    lastBid = data.Data.BestBidPrice;
                }
                else if (data.Data.BestAskPrice < lastAsk)
                {
                    Console.WriteLine($"Price of {symbol} in Bitfinex: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    //await SellAsync(symbol, MainBot.myQuantity, data.Data.BestAskPrice);
                    lastBid = data.Data.BestBidPrice;
                }
            });
        }

        static async Task GetAccountInfo(BitfinexClient client)
        {


            var accountInfo = await client.SpotApi.Account.GetBalancesAsync();

            if (accountInfo.Success)
            {
                foreach (var balance in accountInfo.Data)
                {
                    Console.WriteLine($"{balance.Asset} balance: {balance.Total}");
                }
            }
            else
            {
                Console.WriteLine($"Error getting account info: {accountInfo.Error}");
            }
        }
        static async Task PlaceOrderAsync(string mySymbol, OrderSide requiredSide, decimal requiredQuantity, decimal requiredPrice)
        {
            var apiCredentials = GetApiCredentials();
            var client = GetBitfinexClient(apiCredentials);

            var result = await client.SpotApi.Trading.PlaceOrderAsync
            (
                symbol: mySymbol,
                side: requiredSide,
                type: OrderType.ExchangeLimit,
                quantity: requiredQuantity,
                price: requiredPrice
            );

            if (result.Success)
            {
                Console.WriteLine($"Order placed. Order ID: {result.Data.Id}");
            }
            else
            {
                Console.WriteLine($"Error placing order: {result.Error}");
            }
        }

        static async Task BuyAsync(string symbol, decimal quantity, decimal price)
        {
            await PlaceOrderAsync(symbol, OrderSide.Buy, quantity, price);
        }

        static async Task SellAsync(string symbol, decimal quantity, decimal price)
        {
            await PlaceOrderAsync(symbol, OrderSide.Sell, quantity, price);
        }
        static BitfinexSocketClient GetBitfinexSocketClient(ApiCredentials apiCredentials)
        {
            var client = new BitfinexSocketClient(new BitfinexSocketClientOptions()
            {
                ApiCredentials = apiCredentials

            });
            return client;
        }
        static BitfinexClient GetBitfinexClient(ApiCredentials apiCredentials)
        {
            var client = new BitfinexClient(new BitfinexClientOptions()
            {
                ApiCredentials = apiCredentials,
            });
            return client;
        }
    }
}
