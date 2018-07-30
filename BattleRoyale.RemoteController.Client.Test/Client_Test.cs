using Xunit;
using Models = BatteRoyale.RemoteController.Client.Models;

namespace BattleRoyale.RemoteController.Client.Test
{
    /// <summary>
    /// Run With administrator privileges (needs to access the registry for the .Net Version)
    /// </summary>
    public class Client_Test
    {
        
        [Fact]
        public void ShouldBuildClientInformation()
        {
            var client = new Models.Client();

            Assert.NotNull(client);
            Assert.NotEmpty(client.MachineName);
            Assert.NotEmpty(client.LocalIPAddress);
            Assert.NotNull(client.WindowsSpecs);
            Assert.True(!string.IsNullOrEmpty(client.WindowsSpecs.Version));
            Assert.True(!string.IsNullOrEmpty(client.WindowsSpecs.ServicePack));
            Assert.NotNull(client.HardDriveInformation);
            Assert.True(!string.IsNullOrEmpty(client.InstalledDotNetVersion));
        }
    }
}
