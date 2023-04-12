using System;
using System.Net.WebSockets;
using WebSocketSharp;

class Program
{
    static void Main(string[] args)
    {
        var symbol = "XBT/USD";
        var ws = new WebSocketSharp.WebSocket($"wss://ws.kraken.com");

        ws.OnMessage += (sender, e) =>
        {
            Console.WriteLine($"Received message: {e.Data}");
            // здесь можно обрабатывать данные
        };

        ws.OnError += (sender, e) =>
        {
            Console.WriteLine($"Error: {e.Message}");
        };

        ws.OnClose += (sender, e) =>
        {
            Console.WriteLine($"Closed: {e.Reason}");
        };

        ws.OnOpen += (sender, e) =>
        {
            Console.WriteLine($"Connected to WebSocket");
            ws.Send("{\"event\":\"subscribe\", \"pair\":[\"XBT/USD\"], \"subscription\":{\"name\":\"ticker\"}}");
        };

        ws.Connect();

        Console.ReadLine();
    }
}
