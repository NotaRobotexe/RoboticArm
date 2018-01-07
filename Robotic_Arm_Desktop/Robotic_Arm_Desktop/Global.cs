using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robotic_Arm_Desktop
{
    public static class Global
    {
        public static bool StreamOn = false;
        public static System.Windows.Media.Imaging.BitmapSource Frame;
        public static string ipaddres = "169.254.121.6";
        public static bool connected = false;
        public static bool WrongMode = false;
        public static string FfmpegPath = @"C:\Users\mt2si\Desktop\ffmpeg\ffmpeg.exe";

        public static int StreamWidth = 900;
        public static int StreamHight = 600;

        public static bool stop = false; // manual mode stop all action
        public static bool triggered = false;
        public static bool autoModeRunning = false;

        public static short BetterMessageBoxErrorIndex = 0;
        public static bool BetterMessageBoxLauched = false;

        //debug only 
        public static bool DebugMode = true;
    }
}
