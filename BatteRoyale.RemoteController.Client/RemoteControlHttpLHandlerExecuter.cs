using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteRoyale.RemoteController.Client
{
    public class RemoteControlHttpLHandlerExecuter
    {
        public CmdExecuter CmdExecuter { get; set; }

        /// <summary>
        /// This Machine information. The http listener starts listening in the [LocalIPAddress]
        /// </summary>
        public Models.Client ClientInformation { get; set; }

        /// <summary>
        /// The instance of the HttpListenerHandler
        /// </summary>
        public RemoteControlHttpHandler ListenerHandler { get; set; }


        /// <summary>
        /// The logger will log any hits on the listener, or errors occurred
        /// </summary>
        public EventLog Logger { get; set; }

        public RemoteControlHttpLHandlerExecuter(EventLog logger)
        {
            // defines all the informaction of the client
            ClientInformation = new Models.Client();

            // generates an instance of the Command Line executer
            CmdExecuter = new CmdExecuter();

            Logger = logger;
        }

        /// <summary>
        /// Define the URIs you wish to listen. Starts listening the server for command line commands.
        /// </summary>
        public void Start()
        {
            var machineLocation = $"http://{ClientInformation.LocalIPAddress}";
            var serverLocation = ConfigurationManager.AppSettings["BattleRoyaleRemoteControlServerLocation"].ToString();
            var serverUriRegisterClient = ConfigurationManager.AppSettings["BattleRoyaleRemoteControlServerUriRegisterClient"].ToString();

            try
            {

                ListenerHandler = new RemoteControlHttpHandler(machineLocation)
                    .OnConnect(serverLocation+ serverUriRegisterClient, ClientInformation, result =>
                    {
                        if(!result.Success)
                        {
                            Logger.WriteEntry(result.Message, EventLogEntryType.Error);
                        }
                    })
                    .AddListener("/handshake", requestInfo =>
                    {
                        Logger.WriteEntry("Server calling /handshake uri. Sending back true to indicate that the service is up and running", type: EventLogEntryType.Information);

                        return true;
                    })
                    .AddListener("/receivecommand", requestInfo =>
                    {

                        if (requestInfo.HttpMethod == "GET")
                        {
                            Logger.WriteEntry("Server calling /receivecommand uri, but the httpMethod was GET. Refusing connection, returning error 400", type: EventLogEntryType.Warning);
                            return null;
                        
                        }

                        Logger.WriteEntry($"Server calling /receivecommand uri. The Command received was {requestInfo.BodyString}", type: EventLogEntryType.Information);

                        var output = CmdExecuter.ExecuteCommand(requestInfo.BodyString);

                        if(!output.Success)
                        {
                            Logger.WriteEntry($"Error reported while executing command '{requestInfo.BodyString}': {output.Error}", type: EventLogEntryType.Error);
                        }

                        return output;
                    })
                    .AddListener("/clientinformation", requestInfo =>
                    {
                        Logger.WriteEntry("Server calling /clientinformation uri. Sending back the client information.", type: EventLogEntryType.Information);

                        if (requestInfo.HttpMethod == "POST")
                            return null;

                        return ClientInformation;
                    })
                    .Start();

                Logger.WriteEntry("Service up and running at "+ machineLocation, type: EventLogEntryType.Information);

            } catch (Exception)
            {
                Logger.WriteEntry($"An error occurred in the starting the service", type: EventLogEntryType.Error);
            }

        }

        /// <summary>
        /// Stops the task in which the Listener is executing and remove it's instance, until the user starts the service again
        /// </summary>
        public void Stop()
        {
            if(ListenerHandler != null)
            {
                ListenerHandler.Stop();
            }
        }
    }
}
