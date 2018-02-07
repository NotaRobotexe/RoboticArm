using System;
using System.Diagnostics;
using System.IO;

namespace RemoteExecution
{
    class Program
    {
        static Process python;
        static Network network;
        static bool SciptRunning;

        static void Main(string[] args)
        {
            relaunch:
            network = new Network();
            network.init();
            while (true)
            {
                network.RecieveData();
               
                if (RunORStopScript()==false){
                    network.Release();
                    goto relaunch;
                }
            }
        }

        static bool RunORStopScript()
        {
            if (network.raw != "")
            {
                if (network.raw.Substring(0,3)=="END" && SciptRunning==true)
                {
                    python.Close();
                    SciptRunning = false;
                }
                else
                {
                    File.WriteAllText("Script.py", network.raw);
                    StartScript();
                    network.sendACK();
                }
            return true;
            }
            else{
                return false;
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
            SciptRunning = true;
        }

        private static void Python_Exited(object sender, EventArgs e)
        {
            SciptRunning = false;
        }

    }
}
