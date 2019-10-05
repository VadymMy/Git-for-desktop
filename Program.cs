using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GitAutomation
{
    class Program
    {
        private static readonly Dictionary<string, string> Commands = new Dictionary<string, string>
        {
            { "Status", "git status" },
            { "Add", "git add *.*" },
            { "Commit", $"git commit -m \"{DateTime.Now.ToString()}\"" },
            { "Push", "git push" },
            { "Exit", "exit" }
        };
        private static ProcessStartInfo StartInfo;
        private static Process Process;
        private static Exception[] exceptions = new Exception[]
        {
            new Exception("It is not any repository in this directory."),
            new Exception("No changes, nothing to commit."),
            new Exception("No internet connection. Try to push commits later.")
        };
        private static string currentLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        private static void SetStartInfo()
        {
            StartInfo = new ProcessStartInfo();
            StartInfo.FileName = "cmd.exe";
            StartInfo.WorkingDirectory = currentLocation;
            StartInfo.UseShellExecute = false;
            StartInfo.CreateNoWindow = true;
            StartInfo.RedirectStandardInput = true;
            StartInfo.RedirectStandardOutput = true;
        }
        private static void RunProcess()
        {
            Process = new Process();
            Process.StartInfo = StartInfo;
            Process.Start();
        }

        private static bool IsInternetConnected()
        {
            InternetAvailability internet = new InternetAvailability();
            internet.CheckConnection();
            if ((bool)internet.IsConnected)
                return true;
            else
                return false;
        }
        private static bool IsGitRepoInDirectory(string statusInput)
        {
            if (statusInput.Contains("branch")) return true;
            return false;
        }
        private static bool IsSmthToCommit(string statusInput)
        {
            if (!statusInput.Contains("nothing to commit")) return true;
            return false;
        }
        private static bool ExistsNotPushedCommits(string statusInput)
        {
            if (statusInput.Contains("ahead")) return true;
            return false;
        }
        private static string EnterCommand(string command)
        {
            Console.WriteLine("Processing...");
            RunProcess();
            Process.StandardInput.WriteLine(command);
            Process.StandardInput.Close();
            string input = Process.StandardOutput.ReadToEnd();
            Process.Close();
            return input;
        }
        private static void Push()
        {
            if (IsInternetConnected())
            {
                EnterCommand((Commands["Push"]));
                Console.WriteLine("Processed.");
            }
            else
                Console.WriteLine(exceptions[2].Message);
        }
        static void Main(string[] args)
        {
            SetStartInfo();
            string statusInput = EnterCommand((Commands["Status"]));
            if (!IsGitRepoInDirectory(statusInput))
                Console.WriteLine(exceptions[0].Message);
            else if (!IsSmthToCommit(statusInput) && !ExistsNotPushedCommits(statusInput))
                Console.WriteLine(exceptions[1].Message);
            else if(!IsSmthToCommit(statusInput) && ExistsNotPushedCommits(statusInput))            
                Push();            
            else
            {
                EnterCommand((Commands["Add"]));
                EnterCommand((Commands["Commit"]));
                Push();
            }
            Console.ReadKey();
        }
    }
}
