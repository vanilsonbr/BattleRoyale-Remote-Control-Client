using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteRoyale.RemoteController.Client.Models
{
    public class CommandResult
    {
        public string WorkingDirectory { get; set; }
        public string Result { get; set; }
        public string CommandSent { get; set; }
        public string Error { get; internal set; }
        public bool Success { get; internal set; }
    }
}
