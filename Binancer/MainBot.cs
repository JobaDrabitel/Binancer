using Binance.Net.Clients;
using Binance.Net.Objects;
using Binance.Net.Enums;
using CryptoBot;
using CryptoExchange.Net.CommonObjects;

namespace Binancer
{

    class MainBot
    {
     
        public static decimal myQuantity = 0.001m;
        static async Task Main(string[] args)
        {     
            await BittrexBot.BittrexCheckAsync();
            await BinanceBot.BinanceCheckAsync();
           
            while (true) { }
        }
        
    }

}

