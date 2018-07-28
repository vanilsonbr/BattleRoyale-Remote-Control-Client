using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Threading;
using BatteRoyale.RemoteController.Client.Models;

namespace BatteRoyale.RemoteController.Client
{
    public class RemoteControlHttpHandler
    {
        private HttpListener _listener;
        private Dictionary<string, Func<RequestInfo, object>> _mapPathToAction;
        private List<Action> _messagesOnConnect;
        private bool _sendMessageOnConnect;
        private string _baseUrl;
        private Thread _serverThread;

        public RemoteControlHttpHandler(string baseUrl)
        {
            _mapPathToAction = new Dictionary<string, Func<RequestInfo, object>>();
            _listener = new HttpListener();
            _baseUrl = baseUrl;
            _sendMessageOnConnect = false;
        }

        public RemoteControlHttpHandler Start(Action<HandlerResult> callback = null)
        {
            _serverThread = new Thread(this.StartListener);
            _serverThread.Start();
            return this;
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void StartListener()
        {
            // executing all the requests created by the user before the server begins to listen
            if (_sendMessageOnConnect)
            {
                foreach (var action in _messagesOnConnect)
                {
                    action();
                }
            }

            this._listener.Start();

            

            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = this.AddResponseHeaders(context.Response);

                // mount request objects required
                var requestInfo = new RequestInfo(request);

                // take the action for that uri
                var action = _mapPathToAction[context.Request.Url.AbsolutePath.ToLower()];

                // execute the Func Handler for that uri
                object responseObject = action(requestInfo);
                if(responseObject == null)
                {
                    // send back BadRequest
                    response.StatusCode = 400;
                } 

                string responseString = responseObject != null? JsonConvert.SerializeObject(responseObject) : "";

                // send reponse back to the requester as a JSON
                SendDataBack(responseString, response);
            }
            //_listener.Stop();
        }

        /// <summary>
        /// Listener builder. Listens to a url and sends information back to the requester
        /// </summary>
        /// <param name="path">the uri</param>
        /// <param name="action">the function that will be execute receiving the request parameters</param>
        public RemoteControlHttpHandler AddListener(string path, Func<RequestInfo, object> action)
        {
            _listener.Prefixes.Add(_baseUrl + path + "/");

            _mapPathToAction.Add(path, action);

            return this;
        }

        /// <summary>
        /// Registers a post http request to be sent upon start to the AbsoluteUri.
        /// </summary>
        /// <typeparam name="T">The type of the Object to be send</typeparam>
        /// <param name="AbsoluteUri">The Uri to where the data will be sent</param>
        /// <param name="dataToSend">the information to be sent. It will be serialized to JSON</param>
        /// <returns>As a builder, returns this instance of RemoteControlHttpListener</returns>
        public RemoteControlHttpHandler OnConnect<T>(string AbsoluteUri, T dataToSend, Action<HandlerResult> handlerResult = null)
        {
            if (_messagesOnConnect == null)
            {
                _messagesOnConnect = new List<Action>();
                _sendMessageOnConnect = true;
            }

            _messagesOnConnect.Add(() =>
            {
                HttpClient client = new HttpClient();
                var dataToSendJson = JsonConvert.SerializeObject(dataToSend);

                var content = new StringContent(dataToSendJson, Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(AbsoluteUri, content).Result;
                } catch (Exception ex)
                {
                    string errorMessage =
                        "The Server is unreachable." +
                        " Cannot send registration message to the Remote Control Server." +
                        " This may be caused by the Server being offline. " +
                        " This client machine may not be controlled by the Server." +
                        " Please check the Server configuration and the Server status";

                    handlerResult?.Invoke(new HandlerResult
                    {
                        Success = false,
                        Message = errorMessage
                    });
                }
            });

            return this;
        }

        private void SendDataBack(string responseString, HttpListenerResponse response)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        private HttpListenerResponse AddResponseHeaders(HttpListenerResponse response)
        {
#if DEBUG
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            response.Headers.Add("Access-Control-Allow-Methods", "POST, GET");
#endif
            return response;
        }
    }
}
