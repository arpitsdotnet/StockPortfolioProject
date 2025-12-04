using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.TimeSeriesWeekly;

public sealed record TimeSeriesWeeklyRequest(string Symbol, string Month);
public sealed record TimeSeriesWeeklyResponse(string SeriesDateTime, decimal Open, decimal High, decimal Low, decimal Close, long Volumne);

public class TimeSeriesWeeklyHandler
{
    private readonly IStockApiClient _stockApiClient;

    public TimeSeriesWeeklyHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }

    public async Task<Result<List<TimeSeriesWeeklyResponse>>> Handle(TimeSeriesWeeklyRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.TIME_SERIES_WEEKLY}&" +
            $"symbol={request.Symbol}&" +
            $"month={request.Month}";

        var apiResponse = await _stockApiClient.Query<TimeSeriesWeeklyResponse_Body>(query, cancellationToken);
        if (apiResponse.IsFailure)
        {
            return apiResponse.Error;
        }

        List<TimeSeriesWeeklyResponse> response = apiResponse.Value.GetTimeSeriesItemList();
        if (response == null || response.Count <= 0)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Response is empty.");
        }

        return Result<List<TimeSeriesWeeklyResponse>>.Success(response);
    }

    internal class TimeSeriesWeeklyResponse_Body
    {
        [JsonPropertyName("Meta Data")]
        public TimeSeries_MetaDataResponse MetaData { get; set; }

        [JsonPropertyName("Weekly Time Series")]
        public Dictionary<string, TimeSeries_ItemResponse> TimeSeriesWeekly { get; set; }

        public List<TimeSeriesWeeklyResponse> GetTimeSeriesItemList() => [.. GetTimeSeriesItemListFromDict(TimeSeriesWeekly)];

        private IEnumerable<TimeSeriesWeeklyResponse> GetTimeSeriesItemListFromDict(Dictionary<string, TimeSeries_ItemResponse> apiResponseDict) =>
            apiResponseDict.Select(item =>
            new TimeSeriesWeeklyResponse(
                item.Key,
                item.Value.Open,
                item.Value.High,
                item.Value.Low,
                item.Value.Close,
                item.Value.Volume));

    }

}
