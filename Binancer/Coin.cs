using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot
{
    class Coin
    {
        public required string Symbol { get; set; }
        public required decimal BuyPrice { get; set; }
        public required decimal SellPrice { get; set;}
    }
}
