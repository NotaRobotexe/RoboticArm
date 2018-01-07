using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robotic_Arm_Desktop
{
    class VideoStream
    {
        private Process process;
        private BitmapSource bmp;
        private int bytesRead;
        private bool processKilled = false;

        public static event EventHandler NewFrame;
        //-loglevel quiet
        public void Procesinit()
        {
            if (Global.DebugMode == false)
            {
                process = new Process();
                process.StartInfo.FileName = Global.FfmpegPath;
                process.StartInfo.Arguments = @"-i rtsp://"+Global.ipaddres+":8554/unicast  -c:v copy -r 60 -f image2pipe -pix_fmt rgb24 -vcodec rawvideo -";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                ffmpegReader();
            }
        }

        private void ffmpegReader()
        {
            Thread thread = new Thread(() =>
            {
                var bytesPerPixel = (PixelFormats.Rgb24.BitsPerPixel + 7) / 8;
                var stride = bytesPerPixel * Global.StreamWidth;

                int bufferSize = Global.StreamWidth * Global.StreamHight * 3;
                const int maxFfmpegBufferSize = 32768;
                byte[] buffer = new byte[bufferSize];
                byte[] rawBuffer = new byte[maxFfmpegBufferSize];     //this is max size of buffer what will ffmpeg return
                int bytesReaded = 0;
                var ffmpegOut = process.StandardOutput.BaseStream;
                bytesRead = 0;

                byte NumberOfFullRawBuffers = (byte)Math.Floor((double)bufferSize / (double)maxFfmpegBufferSize);
                int LastRawBufferSize = bufferSize - (NumberOfFullRawBuffers * maxFfmpegBufferSize);

                bool ErrorCatch = false;
                using (BinaryReader reader = new BinaryReader(ffmpegOut))   
                {
                    do
                    {
                        IncorectValue:
                        for (int i = 0; i < NumberOfFullRawBuffers; i++)
                        {
                            if (ErrorCatch == false)
                            {
                                bytesRead = reader.Read(rawBuffer, 0, maxFfmpegBufferSize);
                                Array.Copy(rawBuffer, 0, buffer, bytesReaded, bytesRead);
                            }
                            else
                            {
                                Array.Copy(rawBuffer, 0, buffer, bytesReaded, bytesRead);
                                ErrorCatch = false;
                            }
                            bytesReaded += bytesRead;
                        }
                        bytesRead = reader.Read(rawBuffer, 0, maxFfmpegBufferSize);
                        if (bytesRead == LastRawBufferSize)
                        {
                            Array.Copy(rawBuffer, 0, buffer, bytesReaded, bytesRead);
                            bytesReaded = 0;

                        }
                        else
                        {
                            if (processKilled==true){
                                break;
                            }
                            ErrorCatch = true;
                            Console.WriteLine("error ");
                            bytesReaded = 0;
                            goto IncorectValue;
                        }
                        if (processKilled==true){
                            break;
                        }

                        bmp = BitmapImage.Create(Global.StreamWidth, Global.StreamHight, 96, 96, PixelFormats.Rgb24, null, buffer, stride);
                        bmp.Freeze();

                        Global.Frame = bmp; 
                        OnNewFrame(EventArgs.Empty);

                        GC.Collect(); //for whatever reason garbage collection wont free memory but when i call him it W.O.R.K.S && and memory usage is miracle low as mine will to live  
                    } while (bytesRead != 0);
                }
            });
            Console.WriteLine("error sema");
            thread.Start();
        }

        public void ProcesEnd()
        {
            if (Global.DebugMode == false)
            {
                processKilled = true;
                process.Kill();
                process.Dispose();
                GC.Collect();
            }
        }

        protected virtual void OnNewFrame(EventArgs e)
        {
            EventHandler eventHandler = NewFrame;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }

    }
}
