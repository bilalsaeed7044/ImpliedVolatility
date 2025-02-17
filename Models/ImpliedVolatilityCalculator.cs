using ImpliedVolatility.Models;
using MathNet.Numerics;
public static class ImpliedVolatilityCalculator
{
    private const double Tolerance = 1e-5;
    private const int MaxIterations = 100;

    public static double CalculateImpliedVolatility(OptionParameters option)
    {
        double sigma = 0.2; // Initial volatility guess
        int iteration = 0;

        while (iteration < MaxIterations)
        {
            double price = BlackScholesFormula(option, sigma);
            double vega = Vega(option, sigma);

            if (vega == 0) break; // Prevent division by zero

            double diff = price - option.MarketPrice;
            if (Math.Abs(diff) < Tolerance) return sigma; // Converged

            sigma -= diff / vega; // Newton-Raphson update
            iteration++;
        }

        return sigma; // Return last computed volatility
    }

    private static double BlackScholesFormula(OptionParameters option, double sigma)
    {
        double d1 = (Math.Log(option.UnderlyingPrice / option.StrikePrice) +
                    (option.RiskFreeRate - option.DividendYield + 0.5 * sigma * sigma) * option.TimeUntilExpiration)
                    / (sigma * Math.Sqrt(option.TimeUntilExpiration));

        double d2 = d1 - sigma * Math.Sqrt(option.TimeUntilExpiration);

        if (option.OptionType == OptionType.Call)
        {
            return option.UnderlyingPrice * Math.Exp(-option.DividendYield * option.TimeUntilExpiration) * CND(d1)
                 - option.StrikePrice * Math.Exp(-option.RiskFreeRate * option.TimeUntilExpiration) * CND(d2);
        }
        else
        {
            return option.StrikePrice * Math.Exp(-option.RiskFreeRate * option.TimeUntilExpiration) * CND(-d2)
                 - option.UnderlyingPrice * Math.Exp(-option.DividendYield * option.TimeUntilExpiration) * CND(-d1);
        }
    }

    private static double Vega(OptionParameters option, double sigma)
    {
        double d1 = (Math.Log(option.UnderlyingPrice / option.StrikePrice) +
                    (option.RiskFreeRate - option.DividendYield + 0.5 * sigma * sigma) * option.TimeUntilExpiration)
                    / (sigma * Math.Sqrt(option.TimeUntilExpiration));

        return option.UnderlyingPrice * Math.Exp(-option.DividendYield * option.TimeUntilExpiration) * NormalPDF(d1)
               * Math.Sqrt(option.TimeUntilExpiration);
    }

    private static double CND(double x)
    {
        return (1.0 + SpecialFunctions.Erf(x / Math.Sqrt(2.0))) / 2.0;
    }

    private static double NormalPDF(double x)
    {
        return Math.Exp(-0.5 * x * x) / Math.Sqrt(2.0 * Math.PI);
    }
}
