using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace listenerTest
{
    public enum RequestType { ImageUpload, KiosksAvailable, FoundServer };
    class HttpResponse
    {
        public Boolean Success { get; }
        public string RequestType { get; }
        public List<string> Data { get;}

        public HttpResponse(Boolean success, RequestType requestType, List<string> data)
        {
            Success = success;
            RequestType = requestType.ToString();
            Data = data;
        }
    }
}
