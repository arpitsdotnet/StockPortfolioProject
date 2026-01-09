namespace StockPortfolio.WebUI.Models.BaseModels
{
    public sealed class Error
    {
        public ErrorType ErrorType { get; set; }
        public string Code { get; set; }
        public string? Description { get; set; } = null;

        public static readonly Error None = new Error(ErrorType.NONE, string.Empty);

        public Error(ErrorType errorType, string code, string? description = null)
        {
            ErrorType = errorType;
            Code = code;
            Description = description;
        }
    }

    public enum ErrorType
    {
        NONE = 0,
        VALIDATION = 1,
        FAILURE = 2
    }
    public class ErrorCode
    {
        public const string NONE = "000";
        public const string BAD_REQUEST = "400";
        public const string UNAUTHORIZED = "401";
        public const string PAYMENT_REQUIRED = "402";
        public const string FORBIDDEN = "403";
        public const string NOT_FOUND = "404";
        public const string METHOD_NOT_ALLOWED = "405";
        public const string NOT_ACCEPTABLE = "406";
        public const string PROXY_AUTHENTICATION_REQUIRED = "407";
        public const string REQUEST_TIMEOUT = "408";
        public const string CONFLICT = "409";
        public const string GONE = "410";
        public const string LENGTH_REQUIRED = "411";
        public const string PRECONDITION_FAILED = "412";
        public const string CONTENT_TOO_LARGE = "413";
        public const string URI_TOO_LONG = "414";
        public const string UNSUPPORTED_MEDIA_TYPE = "415";
        public const string RANGE_NOT_SATISFIABLE = "416";
        public const string EXPECTATION_FAILED = "417";
        public const string MISDIRECTED_REQUEST = "421";
        public const string UNPROCESSABLE_CONTENT = "422";
        public const string LOCKED = "423";
        public const string FAILED_DEPENDENCY = "424";
        public const string TOO_EARLY = "425";
        public const string UPGRADE_REQUIRED = "426";
        public const string PRECONDITION_REQUIRED = "428";
        public const string TOO_MANY_REQUESTS = "429";
        public const string REQUEST_HEADER_FIELDS_TOO_LARGE = "431";
        public const string UNAVAILABLE_FOR_LEGAL_REASONS = "451";
        public const string INTERNAL_SERVER_ERROR = "500";
        public const string NOT_IMPLEMENTED = "501";
        public const string BAD_GATEWAY = "502";
        public const string SERVICE_UNAVAILABLE = "503";
        public const string GATEWAY_TIMEOUT = "504";
        public const string HTTP_VERSION_NOT_SUPPORTED = "505";
        public const string VARIANT_ALSO_NEGOTIATES = "506";
        public const string INSUFFICIENT_STORAGE = "507";
        public const string LOOP_DETECTED = "508";
        public const string NOT_EXTENDED = "510";
        public const string NETWORK_AUTHENTICATION_REQUIRED = "511";
    }
}
