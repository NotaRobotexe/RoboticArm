using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    public static class Global
    {
        public static string ipaddres = "169.254.223.188";

        public static bool stop = false; // manual mode stop all action
        public static bool triggered = false;
        public static bool autoModeRunning = false;
        public static bool conectionRefused = false;
    }
}
