using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    public static class Global
    {
        public static string ipaddres = "169.254.48.106";
        public static bool connected = false;


        public static bool stop = false; // manual mode stop all action
        public static bool triggered = false;
        public static bool autoModeRunning = false;

        //debug only 
        public static bool DebugMode = true;
        public static bool OfflineVideo = false;
    }
}
