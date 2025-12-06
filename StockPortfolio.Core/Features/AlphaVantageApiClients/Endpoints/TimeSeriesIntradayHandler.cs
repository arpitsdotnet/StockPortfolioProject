using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

public sealed record TimeSeriesIntradayRequest(string Symbol, string Interval, string Month = "");
public sealed record TimeSeriesIntradayResponse(string SeriesDateTime, decimal Open, decimal High, decimal Low, decimal Close, long Volumne);

public class TimeSeriesIntradayHandler
{
    private readonly IStockApiClient _stockApiClient;

    public TimeSeriesIntradayHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }

    public async Task<Result<List<TimeSeriesIntradayResponse>>> Handle(TimeSeriesIntradayRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.TIME_SERIES_INTRADAY}&" +
            $"symbol={request.Symbol}&" +
            $"interval={request.Interval}";

        if (!string.IsNullOrWhiteSpace(request.Month))
            query += $"month={request.Month}";

        var apiResponse = await _stockApiClient.Query<TimeSeriesIntradayResponse_Body>(query, cancellationToken);
        if (apiResponse.IsFailure)
        {
            return apiResponse.Error;
        }
        if (!string.IsNullOrEmpty(apiResponse.Value.ErrorMessage))
        {
            return new Error(ErrorType.FAILURE, ErrorCode.INTERNAL_SERVER_ERROR, apiResponse.Value.ErrorMessage);
        }

        List<TimeSeriesIntradayResponse> response = apiResponse.Value.GetTimeSeriesItemList();
        if (response == null || response.Count <= 0)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Response is empty.");
        }

        return Result<List<TimeSeriesIntradayResponse>>.Success(response);
    }

    internal class TimeSeriesIntradayResponse_Body
    {
        [JsonPropertyName("Error Message")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("Meta Data")]
        public TimeSeries_MetaDataResponse? MetaData { get; set; }

        [JsonPropertyName("Time Series (1min)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeries1min { get; set; }

        [JsonPropertyName("Time Series (5min)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeries5min { get; set; }

        [JsonPropertyName("Time Series (15min)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeries15min { get; set; }

        [JsonPropertyName("Time Series (30min)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeries30min { get; set; }

        [JsonPropertyName("Time Series (60min)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeries60min { get; set; }

        public List<TimeSeriesIntradayResponse> GetTimeSeriesItemList() => MetaData?.Interval switch
        {
            "1min" => [.. GetTimeSeriesItemListFromDict(TimeSeries1min!)],
            "5min" => [.. GetTimeSeriesItemListFromDict(TimeSeries5min!)],
            "15min" => [.. GetTimeSeriesItemListFromDict(TimeSeries15min!)],
            "30min" => [.. GetTimeSeriesItemListFromDict(TimeSeries30min!)],
            "60min" => [.. GetTimeSeriesItemListFromDict(TimeSeries60min!)],
            _ => new List<TimeSeriesIntradayResponse>(),
        };

        private IEnumerable<TimeSeriesIntradayResponse> GetTimeSeriesItemListFromDict(Dictionary<string, TimeSeries_ItemResponse> dict) =>
            dict.Select(item =>
            new TimeSeriesIntradayResponse(
                item.Key,
                item.Value.Open,
                item.Value.High,
                item.Value.Low,
                item.Value.Close,
                item.Value.Volume));
    }

}
