using ImpliedVolatility.Models;
using System.Data.SqlClient;

namespace ImpliedVolatility.DB
{
    public static class Upsert
    {
        public static List<OptionParameters> FetchOptionsData(string connectionString)
        {
            List<OptionParameters> options = new List<OptionParameters>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
        SELECT OptionID, UnderlyingPrice, StrikePrice, DaysUntilExpiration / 365.0 AS TimeUntilExpiration,
               RiskFreeRate / 100.0 AS RiskFreeRate, DividendYield / 100.0 AS DividendYield, 
               MarketPrice, CallPut
        FROM OptionsPricingData
        WHERE DaysUntilExpiration > 0;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Extract each value individually
                        int optionID = reader.GetInt32(0);

                        double underlyingPrice = reader.IsDBNull(1) ? 0 : Convert.ToDouble(reader.GetDecimal(1)); // Decimal to Double

                        double strikePrice = reader.IsDBNull(2) ? 0 : Convert.ToDouble(reader.GetDecimal(2)); // Decimal to Double

                        double timeUntilExpiration = reader.IsDBNull(3) ? 0 : Convert.ToDouble(reader.GetDecimal(3)); ;

                        double riskFreeRate = reader.IsDBNull(4) ? 0 : Convert.ToDouble(reader.GetDecimal(4)); // Decimal to Double

                        double dividendYield = reader.IsDBNull(5) ? 0 : Convert.ToDouble(reader.GetDecimal(5)); // Decimal to Double

                        double marketPrice = reader.IsDBNull(6) ? 0 : Convert.ToDouble(reader.GetDecimal(6)); // Decimal to Double

                        // OptionType as Call or Put
                        string callPutValue = reader.GetString(7);
                        OptionType optionType = callPutValue == "Call" ? OptionType.Call : OptionType.Put;

                        // Add the data to the list
                        options.Add(new OptionParameters
                        {
                            OptionID = optionID,
                            UnderlyingPrice = underlyingPrice,
                            StrikePrice = strikePrice,
                            TimeUntilExpiration = timeUntilExpiration,
                            RiskFreeRate = riskFreeRate,
                            DividendYield = dividendYield,
                            MarketPrice = marketPrice,
                            OptionType = optionType
                        });
                    }
                }
            }

            return options;
        }




        public static void UpdateImpliedVolatilityBatch(string connectionString, List<OptionParameters> options)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    foreach (var option in options)
                    {
                        string updateQuery = "UPDATE OptionsPricingData SET ImpliedVolatility = @IV WHERE OptionID = @ID;";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@IV", option.ImpliedVolatility);
                            cmd.Parameters.AddWithValue("@ID", option.OptionID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
            }
        }
    }
}
