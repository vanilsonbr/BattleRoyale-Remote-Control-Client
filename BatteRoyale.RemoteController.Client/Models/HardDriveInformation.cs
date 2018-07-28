namespace BatteRoyale.RemoteController.Client.Models
{
    public class HardDriveInformation
    {
        /// <summary>
        /// Total hard drive size in bytes
        /// </summary>
        public long TotalSize { get; set; }
        /// <summary>
        /// Avaiable size in bytes
        /// </summary>
        public long AvailableSize { get; set; }

        /// <summary>
        /// The Name of the HardDrive
        /// </summary>
        public string HardDriveName { get; set; }
    }
}