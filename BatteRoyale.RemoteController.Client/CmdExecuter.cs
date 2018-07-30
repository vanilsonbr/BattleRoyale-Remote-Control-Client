using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

            string nextDirectory = _workingDirectory;
            #region testing if the command is 'cd' and treat it. Verify if the dir (new path) exists]
            if (fetchCommand(arguments) == "cd")
            {
                string path = fetchPathFromCdCommand(arguments);
                nextDirectory = $"{_workingDirectory}\\{path}\\";
                try
                {
                    nextDirectory = Path.GetFullPath(nextDirectory);
                    bool directoryExists = Directory.Exists(nextDirectory);
                    if (!directoryExists)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception ex)
                {
                    // the path doesnt exist
                    return new CommandResult
                    {
                        WorkingDirectory = _workingDirectory,
                        Result = "O sistema não pode encontrar o caminho especificado.",
                        CommandSent = arguments,
                        Success = false
                    };
                }

            }
            #endregion

            try
            {
                // code taken from https://msdn.microsoft.com/en-us/library/system.diagnostics.process.standardoutput.aspx
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.WorkingDirectory = _workingDirectory;
                process.StartInfo.FileName = command;
                process.StartInfo.Arguments = "/C " + arguments;

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                bool success = process.HasExited && process.ExitCode == 0;


                // wait for 20 seconds
                bool exited =  process.WaitForExit(20000);

                _workingDirectory = nextDirectory;

                string result = string.IsNullOrEmpty(error) ? $"\n{output}" : $"{error}{output}";

                return new CommandResult
                {
                    WorkingDirectory = _workingDirectory,
                    CommandSent = arguments,
                    Success = success,
                    Result = result
                };
            }
            catch (Exception ex)
            {
                return new CommandResult
                {
                    WorkingDirectory = _workingDirectory,
                    CommandSent = arguments,
                    Success = false,
                    Result = ex.Message
                };
            }

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
