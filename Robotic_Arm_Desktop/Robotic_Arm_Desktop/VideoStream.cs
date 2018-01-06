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

        public event EventHandler NewFrame;

        public void procesinit()
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

        private void ffmpegReader()
        {
            Thread thread = new Thread(() =>
            {
                var bytesPerPixel = (PixelFormats.Rgb24.BitsPerPixel + 7) / 8;
                var stride = bytesPerPixel * 640;

                int bufferSize = 640 * 480 * 3;
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
                            ErrorCatch = true;
                            Console.WriteLine("error ");
                            bytesReaded = 0;
                            goto IncorectValue;
                        }

                        bmp = BitmapImage.Create(640, 480, 96, 96, PixelFormats.Rgb24, null, buffer, stride);
                        bmp.Freeze();

                        Global.Frame = bmp;
                        OnNewFrame(EventArgs.Empty);


                        GC.Collect(); //for whatever reason garbage collection wont free memory but when i call him it W.O.R.K.S   
                    } while (bytesRead != 0);
                }
            });

            thread.Start();
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
