using ImpliedVolatility.DB;
using ImpliedVolatility.Models;


string connectionString = "Server=CDSNB001;Database=ImpliedVolatility;Trusted_Connection=True;TrustServerCertificate=True";

// Fetch options data
List<OptionParameters> options = Upsert.FetchOptionsData(connectionString);
Console.WriteLine($"Fetched {options.Count} option contracts...");

// Compute IV in parallel
Parallel.ForEach(options, option =>
{
    option.ImpliedVolatility = ImpliedVolatilityCalculator.CalculateImpliedVolatility(option);
});

// Batch update results
Upsert.UpdateImpliedVolatilityBatch(connectionString, options);
Console.WriteLine("Implied volatility calculation completed.");