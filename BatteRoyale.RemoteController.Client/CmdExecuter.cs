using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatteRoyale.RemoteController.Client.Models;

namespace BatteRoyale.RemoteController.Client
{
    public enum CommandShell
    {
        Cmd = 0,
        PowerShell = 1
    };

    public class CmdExecuter
    {
        private string _workingDirectory;

        public CmdExecuter(string workingDirectory = @"C:\")
        {
            _workingDirectory = workingDirectory;
        }

        /// <summary>
        /// Executes the command in cmd.exe and returns the output 
        /// </summary>
        /// <param name="arguments"></param>
        /// <param name="shell"></param>
        /// <returns></returns>
        public CommandResult ExecuteCommand(string arguments, CommandShell shell = CommandShell.Cmd)
        {
            if (string.IsNullOrEmpty(arguments))
                return null;

            string command = shell == CommandShell.Cmd ? "cmd.exe" : "powershell.exe";

            // code taken from https://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = _workingDirectory;
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = "/C " + arguments;

            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            bool success = process.HasExited && process.ExitCode == 0;
            string error = "";

            process.WaitForExit();

            if (fetchCommand(arguments) == "cd")
            {
                if (success)
                {
                    string path = fetchPathFromCdCommand(arguments);
                    _workingDirectory = _workingDirectory + path + @"\";
                }
                else
                {
                    // significa que o caminho especificado não foi encontrado
                    error = "O sistema não pôde encontrar o caminho especificado";
                }
            } else
            {
                if(!success)
                {
                    error = "O sistema não conseguiu executar o comando solicitado";
                }
            }

            return new CommandResult
            {
                WorkingDirectory = _workingDirectory,
                CommandSent = arguments,
                Success = success,
                Result = output,
                Error = error
            };

        }

        private string fetchCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
                return "";

            string[] splitCmdArguments = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return splitCmdArguments[0];
        }

        /// <summary>
        /// when the command executed is a 'cd', gets the argument of the command, which is the path
        /// </summary>
        /// <param name="arguments">the process arguments (cd [path])</param>
        /// <returns>the path</returns>
        private string fetchPathFromCdCommand(string arguments)
        {
            var path = arguments.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1];
            return path;
        }

    }
}
