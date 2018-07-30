namespace BatteRoyale.RemoteController.Client.Models
{
    public class CommandResult
    {
        public string WorkingDirectory { get; set; }
        public string Result { get; set; }
        public string CommandSent { get; set; }
        public bool Success { get; internal set; }
    }
}
