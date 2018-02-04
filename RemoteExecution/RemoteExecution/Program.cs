using System;
using System.Diagnostics;
using System.IO;

namespace RemoteExecution
{
    class Program
    {
        static Process python;
        static Network network;
        static bool relaunch = false;

        static void Main(string[] args)
        {
            begin: //Every day we're fuhrer and fuhrer from God;

            network = new Network();
            network.init();

            while (true)
            {
                network.RecieveData();
                RunORStopScript();
                if (relaunch == true)
                {
                    relaunch = false;
                    goto begin;
                }
            }
        }

        static void RunORStopScript()
        {
            if (network.raw.Substring(0,3)=="END")
            {
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
            python.EnableRaisingEvents = true;
            python.Exited += Python_Exited;
            python.Start();
        }

        private static void Python_Exited(object sender, EventArgs e)
        {

        }

    }
}
