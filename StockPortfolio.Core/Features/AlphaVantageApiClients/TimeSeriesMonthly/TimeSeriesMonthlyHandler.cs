using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.TimeSeriesMonthly;

public sealed record TimeSeriesMonthlyRequest(string Symbol, string Month);
public sealed record TimeSeriesMonthlyResponse(string SeriesDateTime, decimal Open, decimal High, decimal Low, decimal Close, long Volumne);

public class TimeSeriesMonthlyHandler
{
    private readonly IStockApiClient _stockApiClient;

    public TimeSeriesMonthlyHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }

    public async Task<Result<List<TimeSeriesMonthlyResponse>>> Handle(TimeSeriesMonthlyRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.TIME_SERIES_MONTHLY}&" +
            $"symbol={request.Symbol}&" +
            $"month={request.Month}";

        var apiResponse = await _stockApiClient.Query<TimeSeriesMonthlyResponse_Body>(query, cancellationToken);
        if (apiResponse.IsFailure)
        {
            return apiResponse.Error;
        }

        List<TimeSeriesMonthlyResponse> response = apiResponse.Value.GetTimeSeriesItemList();
        if (response == null || response.Count <= 0)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Response is empty.");
        }

        return Result<List<TimeSeriesMonthlyResponse>>.Success(response);
    }

    internal class TimeSeriesMonthlyResponse_Body
    {
        [JsonPropertyName("Meta Data")]
        public TimeSeries_MetaDataResponse MetaData { get; set; }

        [JsonPropertyName("Monthly Time Series")]
        public Dictionary<string, TimeSeries_ItemResponse> TimeSeriesMonthly { get; set; }

        public List<TimeSeriesMonthlyResponse> GetTimeSeriesItemList() => [.. GetTimeSeriesItemListFromDict(TimeSeriesMonthly)];

        private IEnumerable<TimeSeriesMonthlyResponse> GetTimeSeriesItemListFromDict(Dictionary<string, TimeSeries_ItemResponse> apiResponseDict) =>
            apiResponseDict.Select(item =>
            new TimeSeriesMonthlyResponse(
                item.Key,
                item.Value.Open,
                item.Value.High,
                item.Value.Low,
                item.Value.Close,
                item.Value.Volume));

    }

}
