using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Enums;
namespace Binancer
{

    class Binancer
    {
        static BinanceSocketClient? socketClient;
        static readonly string trx = "trxusdt";
        static readonly string[] symbols = new string[] { "btcusdt", "ethusdt", "dogeusdt" };
        static decimal bidForBuy = 0m;
        static decimal askForSell = 0m;
        static decimal myQuantity = 0.001m;
        static async Task Main(string[] args)
        {     
            var apiCredentials = GetBinanceInfo.GetApiCredentials();
            socketClient = GetBinanceSocketClient(apiCredentials);

            await GetCryptoDataAsync(trx);

            Console.ReadLine();
        }
        static async Task GetCryptoDataAsync(string symbol)
        {
            if (socketClient == null) 
                return;

            var depth = await socketClient.SpotStreams.SubscribeToBookTickerUpdatesAsync(symbol, async data =>
            {
                if (data.Data.BestBidPrice < bidForBuy)
                    await SendOrderAsync(trx, myQuantity, data.Data.BestBidPrice, OrderSide.Buy);
                if (data.Data.BestAskPrice < askForSell)
                    await SendOrderAsync(trx, myQuantity, data.Data.BestAskPrice, OrderSide.Sell);
            });

            await GetAccountInfo(GetBinanceInfo.GetBinanceClient(GetBinanceInfo.GetApiCredentials()));
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
            var apiCredentials = GetBinanceInfo.GetApiCredentials();
            var client = GetBinanceInfo.GetBinanceClient(apiCredentials);

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

        static async Task SendOrderAsync(string symbol, decimal quantity, decimal price, OrderSide side) => await PlaceOrderAsync(symbol, side, quantity, price);

        static BinanceSocketClient GetBinanceSocketClient(BinanceApiCredentials apiCredentials)
        {
            var client = new BinanceSocketClient(new BinanceSocketClientOptions()
            {
                ApiCredentials = apiCredentials
            });
            return client;
        }
    }
}