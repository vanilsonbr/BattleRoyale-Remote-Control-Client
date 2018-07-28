using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace BatteRoyale.RemoteController.Client
{
    public partial class RemoteControllerClientService : ServiceBase
    {
        private RemoteControlHttpLHandlerExecuter _httpListenerExecuter;

        public RemoteControllerClientService()
        {
            InitializeComponent();
            if(!EventLog.SourceExists("httpListener"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "httpListener", "Log BattleRoyale RemoteControl");
            }
            HttpListenerEventLog.Source = "httpListener";
            HttpListenerEventLog.Log = "Log BattleRoyale RemoteControl";

            _httpListenerExecuter = new RemoteControlHttpLHandlerExecuter(HttpListenerEventLog);
        }

        protected override void OnStart(string[] args)
        {
            HttpListenerEventLog.WriteEntry("The service is starting");

            _httpListenerExecuter.Start();
        }

        protected override void OnStop()
        {
            _httpListenerExecuter.Stop();

            HttpListenerEventLog.WriteEntry("The service stopped");
        }
    }
}
