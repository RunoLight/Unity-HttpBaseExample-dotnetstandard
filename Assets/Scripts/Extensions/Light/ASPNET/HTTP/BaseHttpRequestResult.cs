using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Light.Unity.AspNetCore.HTTP.Extensions
{
    public abstract class BaseHttpRequestResult : IDisposable
    {
        public BaseHttpRequestResult(HttpResponseMessage response)
        {
            MessageResponse = response;
        }

        public BaseHttpRequestResult(HttpResponseMessage response, Dictionary<string, List<string>> messages) :
            this(response)
        {
            ErrorMessages = messages;
        }

        public BaseHttpRequestResult()
        {
        }

        public HttpResponseMessage MessageResponse { get; set; }

        public Dictionary<string, List<string>> ErrorMessages { get; set; } = new Dictionary<string, List<string>>();

        public bool IsSuccess => MessageResponse.IsSuccessStatusCode;

        public bool IsBadRequest => MessageResponse.StatusCode == HttpStatusCode.BadRequest;

        public void Dispose()
        {
            MessageResponse.Dispose();
        }

        public IEnumerable<string> GetErrorMessages(string key = "")
        {
            ErrorMessages.TryGetValue(key, out var result);
            return result ?? new List<string>();
        }
    }

    public class HttpRequestResult<TData> : BaseHttpRequestResult
    {
        public HttpRequestResult(HttpResponseMessage response, TData data) : base(response)
        {
            Data = data;
        }

        public HttpRequestResult(HttpResponseMessage response) : base(response)
        {
        }

        public HttpRequestResult(HttpResponseMessage response, Dictionary<string, List<string>> messages) : base(
            response, messages)
        {
        }

        public HttpRequestResult()
        {
        }

        public TData Data { get; set; }
    }

    public class HttpRequestResult : BaseHttpRequestResult
    {
        public HttpRequestResult()
        {
        }

        public HttpRequestResult(HttpResponseMessage response) : base(response)
        {
        }

        public HttpRequestResult(HttpResponseMessage response, Dictionary<string, List<string>> messages) : base(
            response, messages)
        {
        }
    }
}