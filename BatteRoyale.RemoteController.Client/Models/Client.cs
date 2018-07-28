using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace BatteRoyale.RemoteController.Client.Models
{
    public class Client
    {
        public string MachineName { get; set; }
        public string LocalIPAddress { get; set; }
        public Antivirus InstalledAntivirus { get; set; }
        public bool IsFirewallActivated { get; set; }
        public WindowsSpecs WindowsSpecs { get; set; }
        public string InstalledDotNetVersion { get; set; }
        public List<HardDriveInformation> HardDriveInformation { get; set; }

        public Client()
        {
            this.WindowsSpecs = new WindowsSpecs();
            this.HardDriveInformation = new List<HardDriveInformation>();
            this.BuildClientInfo();
        }

        private void BuildClientInfo()
        {
            #region [Machine Name]
            this.MachineName = Environment.MachineName;
            #endregion

            #region [Local IP Address (IPV4)]
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            if(ipAddress != null)
            {
                this.LocalIPAddress = ipAddress.ToString();
            }
            #endregion

            #region [Antivirus installed]
            #endregion

            #region [Firewall status]
            #endregion

            #region [OS Commercial Name and Service Pack]
            string OSCommercialName = null;
            try
            {
                OSCommercialName = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", true).GetValue("ProductName").ToString();
            }
            catch (Exception ex)
            {
                // operacao nao permitida, o usuario nao e administrador
            }

            if(OSCommercialName != null)
            {
                this.WindowsSpecs.Version = OSCommercialName;
            }else
            {
                this.WindowsSpecs.Version = Environment.OSVersion.VersionString;
            }
            this.WindowsSpecs.ServicePack = Environment.OSVersion.ServicePack;
            #endregion

            #region [Latest DotNet Version Installed]
            var dotNetFrameworkVersionsInstalled = DotNetVersionInstalled();
            this.InstalledDotNetVersion = dotNetFrameworkVersionsInstalled.LastOrDefault();
            #endregion

            #region [HardDrive informations]
            this.HardDriveInformation = new List<HardDriveInformation>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                this.HardDriveInformation.Add(new HardDriveInformation
                {
                    HardDriveName = drive.Name,
                    AvailableSize = drive.AvailableFreeSpace,
                    TotalSize = drive.TotalSize
                });
            }
            #endregion

        }

        private IEnumerable<string> DotNetVersionInstalled()
        {
            try
            {
                List<string> DotNetInstalledVersions = new List<string>();

                var ndp = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP", true);
                //buscando apenas as keys que contem as versoes do .net framework

                // esse "versions" tem um array de subKeys que começam com v, por exemplo v3.5, v4.0, etc
                // isso indica as versoes do .net que estao instaladas ou são referenciadas por alguma aplicação
                // ou que alguma versão da CLR instalada no sistema usa
                var versions = ndp.GetSubKeyNames().Where(subkey => subkey.StartsWith("v"));

                // a partir destes, buscar apenas aqueles que tem a key "InstallPath" ou alguma subkey que
                // contenha "InstallPath"
                foreach (var version in versions)
                {
                    var installedPath = ndp.OpenSubKey(version).GetValue("InstallPath");
                    if (installedPath == null)
                    {
                        // significa que este subkey "version" não tem um Value "InstallPath".
                        // Vamos olhar dentro de sua subkey para ver se tem
                        // geralmente esse valor "key" abaixo ou é a subKey "Client" ou a "Full", caso o framework nesta versao esteja instalado
                        var versionSubKeys = ndp.OpenSubKey(version);
                        foreach (var key in versionSubKeys.GetSubKeyNames())
                        {
                            var versionInstalledPath = versionSubKeys.OpenSubKey(key).GetValue("InstallPath");
                            var versionVersion = versionSubKeys.OpenSubKey(key).GetValue("Version");

                            if (versionInstalledPath != null && versionVersion != null)
                            {
                                DotNetInstalledVersions.Add("Version " + versionVersion.ToString());
                            }

                        }
                    }
                    else
                    {
                        // significa que esta subkey ja tem o valor "installedPath", e basta agora pegar o valor Version
                        // que tambem esta associado e este subkey
                        var versionVersion = ndp.OpenSubKey(version).GetValue("Version");
                        DotNetInstalledVersions.Add("Version " + versionVersion.ToString());
                    }
                }

                return DotNetInstalledVersions.Distinct();
            }
            catch (Exception ex)
            {
                // provavelmente a aplicação não tem permissao de acesso ao regedit e não é possivel verificar esta informação
                return new List<string>();
            }
        }
    }
}

