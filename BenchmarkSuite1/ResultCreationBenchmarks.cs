using BenchmarkDotNet.Attributes;
using StockPortfolio.Core.BaseModels;

namespace StockPortfolio.Core.Benchmarks
{
    // Benchmark to measure allocation and throughput of Result<T> creation
    public class ResultCreationBenchmarks
    {
        private string _value = "some test value";
        private Error _error = new(ErrorType.FAILURE, ErrorCode.INTERNAL_SERVER_ERROR, "sample error");

        [Benchmark]
        public Result<string> CreateSuccess() => Result<string>.Success(_value);

        [Benchmark]
        public Result<string> CreateFailure() => Result<string>.Failure(_error);
    }
}