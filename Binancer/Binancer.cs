using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Enums;
namespace Binancer
{

    class Binancer
    {
        static readonly string trx = "trxusdt";
        static readonly string[] symbols = new string[] { "btcusdt", "ethusdt", "dogeusdt" };
        static BinanceSocketClient? socketClient;
        static decimal bidForBuy = 0m;
        static decimal askForSell = 0m;
        static decimal myQuantity = 0.001m;
        static async Task Main(string[] args)
        {     
            var apiCredentials = GetApiCredentials();
            socketClient = GetBinanceSocketClient(apiCredentials);

            await GetCryptoDataAsync(trx);
            while (true) { }
        }
        static async Task GetCryptoDataAsync(string symbol)
        {
            if (socketClient == null) { return; }
            var depth = await socketClient.SpotStreams.SubscribeToBookTickerUpdatesAsync(symbol, async data =>
            {
                if (data.Data.BestBidPrice < bidForBuy)
                    await BuyAsync(trx, myQuantity, data.Data.BestBidPrice);
                if (data.Data.BestAskPrice < askForSell)
                    await SellAsync(trx, myQuantity, data.Data.BestAskPrice);
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
                positionSide: PositionSide.Long,
                workingType: WorkingType.Mark,
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
        static BinanceApiCredentials GetApiCredentials()
        {
            var apiCredentials = new BinanceApiCredentials("<API key>", "<secret key>");
            return apiCredentials;
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






//        //if (data.Data.BestBidPrice < prevPrice)
//        //{
//        //    Console.WriteLine($"Price dropped below {prevPrice}: {data.Data.BestBidPrice}");
//        //    prevPrice = data.Data.BestBidPrice;
//        //}
//        //if (data.Data.BestBidPrice > prevPrice)
//        //{
//        //    Console.WriteLine($"Price go up: {prevPrice}: {data.Data.BestBidPrice}");
//        //    prevPrice = data.Data.BestBidPrice;
//        //}

//    Console.ReadLine();

