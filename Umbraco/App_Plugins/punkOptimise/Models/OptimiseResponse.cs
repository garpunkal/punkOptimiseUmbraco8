using Newtonsoft.Json;

namespace punkOptimise.Models
{
    public class OptimiseResponse
    {
        public OptimiseResponse(string message)
        {
            Message = message;
        }

        [JsonProperty(PropertyName = "message")]
        public string Message { get; private set; }

        [JsonProperty(PropertyName = "resultType")]
        public Enums.ResultType ResultType { get; set; }
    }
}