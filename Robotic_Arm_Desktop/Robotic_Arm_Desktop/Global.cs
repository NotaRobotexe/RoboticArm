﻿namespace Robotic_Arm_Desktop
{
    public static class Global
    {
        public static string ScriptTargets = "";

        public static bool RemoteExc = false;
        public static bool stopmovement = false;

        public static int MovingSpeed = 15;
        public static bool ScriptEnabled = false;
        public static bool IsMoving = false;
        public static bool InverseKinematicMovement = false;

        public static bool loadingDone = false;

        public static bool StreamOn = false;
        public static System.Windows.Media.Imaging.BitmapSource Frame;
        public static string ipaddres = "";
        public static bool connected = false;
        public static string FfmpegPath = "ffmpeg.exe"; 

        public static int StreamWidth = 600;
        public static int StreamHight = 480;
        public static float streamratioX;
        public static float streamratioY;


        public static bool stop = false; // manual mode stop all action
        public static bool triggered = false;
        public static bool autoModeRunning = false;

        public static short BetterMessageBoxErrorIndex = 0;
        public static bool BetterMessageBoxLauched = false;

        public static bool Recovery = false;

        public static string ScriptOutput = "";

        public static GamepadState gamepadState;

        //debug only
        public static bool DebugMode = false;

        public static System.Windows.Media.Media3D.Vector3D point;
    }
}