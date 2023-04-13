using Bittrex.Net.Clients;
using Bittrex.Net.Enums;
using Bittrex.Net.Objects;
using Bittrex.Net.Objects.Models;
using CryptoExchange.Net.Authentication;

namespace CryptoBot
{
    class BittrexBot
    {

        static BittrexSocketClient? bittrexSocketClient;
        static readonly Coin[] bittrexCoins = new Coin[]
        {
           new Coin { Symbol = "BTC-USD", BuyPrice = 29500m, SellPrice = 30000m },
           new Coin { Symbol = "ETH-USD", BuyPrice = 1800m, SellPrice = 19000m },
           new Coin { Symbol = "TRX-USD", BuyPrice = 0.06530m, SellPrice = 0.06535m }
        };
        static ApiCredentials GetApiCredentials()
        {
            var apiCredentials = new ApiCredentials("<API key>", "<secret key>");
            return apiCredentials;
        }
        public static async Task BittrexCheckAsync()
        {
            var apiCredentials = GetApiCredentials();
            bittrexSocketClient = GetBittrexSocketClient(apiCredentials);
            for (int i = 0; i < bittrexCoins.Length; i++)
                await GetCryptoDataAsync(bittrexCoins[i].Symbol);
        }
        static async Task GetCryptoDataAsync(string symbol)
        {
            var lastBid = 0m;
            var lastAsk = 0m;
            if (bittrexSocketClient == null) { return; }
            var depth = await bittrexSocketClient.SpotStreams.SubscribeToTickerUpdatesAsync(symbol, async data =>
            {
                if (data.Data.BestBidPrice != lastBid)
                {
                    Console.WriteLine($"Price of {symbol} in Bittrex: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    //await BuyAsync(symbol, myQuantity, data.Data.BestBidPrice);
                    lastBid = data.Data.BestBidPrice;
                }
                else if (data.Data.BestAskPrice < lastAsk)
                {
                    Console.WriteLine($"Price of {symbol} in Bittrex: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    //await SellAsync(symbol, myQuantity, data.Data.BestAskPrice);
                    lastBid = data.Data.BestBidPrice;
                }
            });
        }

        static async Task GetAccountInfo(BittrexClient client)
        {


            var accountInfo = await client.SpotApi.Account.GetAccountAsync();
            var balances = await client.SpotApi.Account.GetBalancesAsync();
            var balanceData = balances.Data;
            if (accountInfo.Success)
            {
                foreach (var balance in balanceData)
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
            var bittrexClient = GetBittrexClient(GetApiCredentials());
            var orderData = await bittrexClient.SpotApi.Trading.PlaceOrderAsync(
                mySymbol,
                requiredSide,
                OrderType.Limit,
                TimeInForce.GoodTillCanceled,
                requiredQuantity,
                requiredPrice);

            if (orderData.Success)
            {
                Console.WriteLine($"Order placed. Order ID: {orderData.Data.Id}");
            }
            else if (orderData.Error != null)
            {
                Console.WriteLine($"Error placing order: {orderData.Error.Message}");
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
        static BittrexSocketClient GetBittrexSocketClient(ApiCredentials apiCredentials)
        {
            var client = new BittrexSocketClient(new BittrexSocketClientOptions()
            {
                ApiCredentials = apiCredentials

            });
            return client;
        }
       
        static BittrexClient GetBittrexClient(ApiCredentials apiCredentials)
        {
            var client = new BittrexClient(new BittrexClientOptions()
            {
                ApiCredentials = apiCredentials,
            });
            return client;
        }
    }
}

