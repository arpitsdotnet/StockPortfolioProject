using System.Text.Json.Serialization;
using StockPortfolio.Core.BaseModels;
using StockPortfolio.Core.Contracts;
using StockPortfolio.Core.Features.AlphaVantageApiClients.Models;

namespace StockPortfolio.Core.Features.AlphaVantageApiClients.Endpoints;

public sealed record TimeSeriesDailyRequest(string Symbol);
public sealed record TimeSeriesDailyResponse(string SeriesDateTime, decimal Open, decimal High, decimal Low, decimal Close, long Volumne);

public class TimeSeriesDailyHandler
{
    private readonly IStockApiClient _stockApiClient;

    public TimeSeriesDailyHandler(IStockApiClient stockApiClient)
    {
        _stockApiClient = stockApiClient;
    }

    public async Task<Result<List<TimeSeriesDailyResponse>>> Handle(TimeSeriesDailyRequest request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return new Error(ErrorType.VALIDATION, ErrorCode.BAD_REQUEST, "Request should not be empty.");
        }

        string query = $"query?" +
            $"apikey={AlphaVantageApiConstants.QueryParameters.API_KEY}&" +
            $"function={AlphaVantageApiConstants.Functions.TIME_SERIES_DAILY}&" +
            $"symbol={request.Symbol}";

        var apiResponse = await _stockApiClient.Query<TimeSeriesDailyResponse_Body>(query, cancellationToken);
        if (apiResponse.IsFailure)
        {
            return apiResponse.Error;
        }
        if (!string.IsNullOrEmpty(apiResponse.Value.ErrorMessage))
        {
            return new Error(ErrorType.FAILURE, ErrorCode.INTERNAL_SERVER_ERROR, apiResponse.Value.ErrorMessage);
        }

        List<TimeSeriesDailyResponse> response = apiResponse.Value.GetTimeSeriesItemList();
        if (response == null || response.Count <= 0)
        {
            return new Error(ErrorType.FAILURE, ErrorCode.NOT_FOUND, "Response is empty.");
        }

        return Result<List<TimeSeriesDailyResponse>>.Success(response);
    }

    internal class TimeSeriesDailyResponse_Body
    {
        [JsonPropertyName("Error Message")]
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("Meta Data")]
        public TimeSeries_MetaDataResponse? MetaData { get; set; }

        [JsonPropertyName("Time Series (Daily)")]
        public Dictionary<string, TimeSeries_ItemResponse>? TimeSeriesDaily { get; set; }

        public List<TimeSeriesDailyResponse> GetTimeSeriesItemList() => [.. GetTimeSeriesItemListFromDict(TimeSeriesDaily!)];

        private IEnumerable<TimeSeriesDailyResponse> GetTimeSeriesItemListFromDict(Dictionary<string, TimeSeries_ItemResponse> apiResponseDict) =>
            apiResponseDict.Select(item =>
            new TimeSeriesDailyResponse(
                item.Key,
                item.Value.Open,
                item.Value.High,
                item.Value.Low,
                item.Value.Close,
                item.Value.Volume));

    }

}
