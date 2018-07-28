using System;
using System.Collections.Specialized;
using System.Net;

namespace BatteRoyale.RemoteController.Client.Models
{
    public class RequestInfo
    {
        public string HttpMethod { get; set; }
        public string ContentType { get; set; }
        public NameValueCollection QueryString { get; set; }
        public string BodyString { get; set; }

        public RequestInfo(HttpListenerRequest request)
        {
            HttpMethod = request.HttpMethod;
            ContentType = request.ContentType;
            QueryString = request.QueryString;

            if (request.HasEntityBody)
            {
                System.IO.Stream body = request.InputStream;
                System.Text.Encoding encoding = request.ContentEncoding;
                System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);
                BodyString = reader.ReadToEnd();
                body.Close();
                reader.Close();
            }
        }
    }
}

