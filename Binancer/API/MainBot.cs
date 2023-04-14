using CryptoBot;

namespace CryptoBot.API;

class MainBot
{

    public static decimal myQuantity = 0.001m;

    static async Task Main(string[] args)
    {
        await MarketsStart();

        Console.ReadLine();
    }

    private static async Task MarketsStart()
    {
        await BittrexBot.BittrexCheckAsync();
        await BinanceBot.BinanceCheckAsync();
        await BitfinexBot.BitfinexCheckAsync();
    }
}

