namespace StockPortfolio.Core.Features.AlphaVantageApiClients;
internal static class AlphaVantageApiConstants
{
    internal struct QueryParameters
    {
        public static string API_KEY => "U6WQSP2K0LXWR9GG";
        
        //INTRADAY        
        public static string INTERVAL_1_MIN => "1min";
        public static string INTERVAL_5_MIN => "5min";
        public static string INTERVAL_15_MIN => "15min";
        public static string INTERVAL_30_MIN => "30min";
        public static string INTERVAL_60_MIN => "60min";
        public static bool ADJUSTED_TRUE => true;
        public static bool ADJUSTED_FALSE => false;
        public static bool EXTENDED_HOURS_TRUE => true;
        public static bool EXTENDED_HOURS_FALSE => false;

        public static string OUTPUTSIZE_COMPACT => "compact";
        public static string OUTPUTSIZE_FULL => "full"; //PREMIUM
        public static string DATA_TYPE_JSON => "JSON"; //JSON | CSV
        public static string DATA_TYPE_CSV => "CSV"; //JSON | CSV


    }
    internal struct Functions
    {
        public static string SYMBOL_SEARCH => "SYMBOL_SEARCH";
        public static string TIME_SERIES_INTRADAY => "TIME_SERIES_INTRADAY";
        public static string TIME_SERIES_DAILY => "TIME_SERIES_DAILY";
        public static string TIME_SERIES_DAILY_ADJUSTED => "TIME_SERIES_DAILY_ADJUSTED";
        public static string TIME_SERIES_WEEKLY => "TIME_SERIES_WEEKLY";
        public static string TIME_SERIES_WEEKLY_ADJUSTED => "TIME_SERIES_WEEKLY_ADJUSTED";
        public static string TIME_SERIES_MONTHLY => "TIME_SERIES_MONTHLY";
        public static string TIME_SERIES_MONTHLY_ADJUSTED => "TIME_SERIES_MONTHLY_ADJUSTED";
        public static string NEWS_SENTIMENT => "NEWS_SENTIMENT";
        public static string INSIDER_TRANSACTIONS => "INSIDER_TRANSACTIONS";
        public static string OVERVIEW => "OVERVIEW";

    }
}
