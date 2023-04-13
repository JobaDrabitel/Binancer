using Binance.Net.Clients;
using Binance.Net.Enums;
using Binance.Net.Objects;
using Binance.Net.Objects.Models.Spot;
using Bittrex.Net.Clients;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CryptoBot;
using Binancer;

namespace CryptoBot
{
     public class BinanceBot
    {
        static BinanceApiCredentials apiCredentials = GetApiCredentials();
        static BinanceSocketClient? binanceSocketClient = GetBinanceSocketClient(apiCredentials);
        static readonly Coin[] binanceCoins = new Coin[]
     {
           new Coin { Symbol = "btcusdt", BuyPrice = 29500m, SellPrice = 30000m },
           new Coin { Symbol = "ethusdt", BuyPrice = 1800m, SellPrice = 19000m },
           new Coin { Symbol = "dogeusdt", BuyPrice = 29500m, SellPrice = 30000m },
           new Coin { Symbol = "trxusdt", BuyPrice = 29500m, SellPrice = 30000m }
     };

        static BinanceApiCredentials GetApiCredentials()
        {
            var apiCredentials = new BinanceApiCredentials("<API key>", "<secret key>");
            return apiCredentials;
        }
        public static async Task BinanceCheckAsync()
        {
            var apiCredentials = GetApiCredentials();
            for (int i = 0; i < binanceCoins.Length; i++)
                await GetCryptoDataAsync(binanceCoins[i].Symbol);
        }
        public static async Task GetCryptoDataAsync(string symbol)
        {
            var lastBid = 0m;
            var lastAsk = 0m;
            if (binanceSocketClient == null) { return; }
            var depth = await binanceSocketClient.SpotStreams.SubscribeToBookTickerUpdatesAsync(symbol, async data =>
            {
                if (data.Data.BestBidPrice != lastBid)
                {
                    Console.WriteLine($"Price of {symbol} in Binance: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    //await BuyAsync(symbol, myQuantity, data.Data.BestBidPrice);
                    lastBid = data.Data.BestBidPrice;
                }
                else if (data.Data.BestAskPrice < lastAsk)
                {
                    Console.WriteLine($"Price of {symbol} in Binance: \nBid price: {data.Data.BestBidPrice}, Ask price: {data.Data.BestAskPrice}");
                    await SellAsync(symbol, MainBot.myQuantity, data.Data.BestAskPrice);
                    lastBid = data.Data.BestBidPrice;
                }
            });
        }

        static async Task GetAccountInfo(BinanceClient client)
        {


            var accountInfo = await client.SpotApi.Account.GetAccountInfoAsync();

            if (accountInfo.Success)
            {
                foreach (var balance in accountInfo.Data.Balances)
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
            var client = GetBinanceClient(apiCredentials);

            var result = await client.UsdFuturesApi.Trading.PlaceOrderAsync
            (
                symbol: mySymbol,
                side: requiredSide,
                type: FuturesOrderType.Limit,
                quantity: requiredQuantity,
                orderResponseType: OrderResponseType.Result,
                timeInForce: TimeInForce.GoodTillCanceled,
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
        static BinanceSocketClient GetBinanceSocketClient(BinanceApiCredentials apiCredentials)
        {
            var client = new BinanceSocketClient(new BinanceSocketClientOptions()
            {
                ApiCredentials = apiCredentials

            });
            return client;
        }
        static BinanceClient GetBinanceClient(BinanceApiCredentials apiCredentials)
        {
            var client = new BinanceClient(new BinanceClientOptions()
            {
                ApiCredentials = apiCredentials,
            });
            return client;
        }
    }
}
