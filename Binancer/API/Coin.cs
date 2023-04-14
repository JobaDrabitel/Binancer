namespace CryptoBot.API;
class Coin
{
    public required string Symbol { get; set; }
    public required decimal BuyPrice { get; set; }
    public required decimal SellPrice { get; set; }
}
