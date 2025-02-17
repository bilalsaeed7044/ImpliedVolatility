using System;
namespace ImpliedVolatility.Models
{
    public class OptionParameters
    {
        public int OptionID { get; set; }
        public double UnderlyingPrice { get; set; }
        public double StrikePrice { get; set; }
        public double TimeUntilExpiration { get; set; }
        public double RiskFreeRate { get; set; }
        public double DividendYield { get; set; }
        public double MarketPrice { get; set; }
        public OptionType OptionType { get; set; }
        public double ImpliedVolatility { get; set; }
    }

    public enum OptionType
    {
        Call,
        Put
    }
}
