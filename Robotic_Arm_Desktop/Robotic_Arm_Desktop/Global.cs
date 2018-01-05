using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    public static class Global
    {
        public static string ipaddres = "169.254.110.51";
        public static bool connected = false;
        public static bool WrongMode = false;

        public static bool stop = false; // manual mode stop all action
        public static bool triggered = false;
        public static bool autoModeRunning = false;

        public static short BetterMessageBoxErrorIndex = 0;
        public static bool BetterMessageBoxLauched = false;

        //debug only 
        public static bool DebugMode = true;
    }
}
