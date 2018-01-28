using System;
using System.Diagnostics;
using System.IO;

namespace RemoteExecution
{
    class Program
    {
        static Process python;
        static Network network;
        static void Main(string[] args)
        {
            network = new Network();
            network.init();

            while (true)
            {
                network.RecieveData();
                RunORStopScript();
            }
        }

        static void RunORStopScript()
        {
            if (network.raw.Substring(0,3)=="END")
            {
                Console.WriteLine(python.HasExited);
                if (python.HasExited)
                {

                }
                python.Close();
            }
            else
            {
                File.WriteAllText("Script.py", network.raw);
                StartScript();
                network.sendACK();
            }
        }

        static void StartScript()
        {
            Console.WriteLine("script");
            python = new Process();
            python.StartInfo.FileName = "Script.py";
            python.StartInfo.UseShellExecute = true;
            python.StartInfo.RedirectStandardOutput = false;
            python.StartInfo.CreateNoWindow = false;
            python.Start();
        }
    }
}
