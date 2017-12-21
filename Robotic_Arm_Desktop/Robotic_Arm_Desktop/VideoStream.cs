using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Robotic_Arm_Desktop
{
    class VideoStream //UNDONE:ten ffmpeg sa randomne od seba vymaze WUT???? dokym sa to nefixne tak vzdicky treba mat kopiu ffmpegu lebo ja uz neviem ako. 
    {   
        private Process process;
        private BitmapSource bmp;
        int bytesRead;

        void OfflineVideoStream()
        {
            if(Global.OfflineVideo == true)
            {
            }
        }

        private void procesinit()
        {
            process = new Process();
            process.StartInfo.FileName = @"C:\Users\mt2si\Desktop\ffmpeg-20171031-88c7aa1-win64-static\ffmpeg-20171031-88c7aa1-win64-static\bin\ffmpeg.exe";
            process.StartInfo.Arguments = @"-i rtsp://169.254.45.53:8554/unicast  -c:v copy -r 200 -f image2pipe -pix_fmt rgb24 -vcodec rawvideo -"; //rtsp://192.168.1.12:8554/unicast
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

                int bufferSize = 900 * 600 * 3;
                int maxFfmpegBufferSize = 32768;
                byte[] buffer = new byte[bufferSize];
                byte[] rawBuffer = new byte[maxFfmpegBufferSize];     //this is max size of buffer what will ffmpeg return
                int bytesReaded = 0;
                var ffmpegOut = process.StandardOutput.BaseStream;
                bytesRead = 0;
                using (BinaryReader reader = new BinaryReader(ffmpegOut))   //UNDONE: chcelo by to dat nejaky security check. ak dojde len o 1 bit menej alebo viac zo ffmpegu vsetko padne. treba sa na to niekedy pozriet
                {
                    do
                    {
                        do
                        {
                            bytesRead = reader.Read(rawBuffer, 0, maxFfmpegBufferSize);
                            Array.Copy(rawBuffer, 0, buffer, bytesReaded, bytesRead);
                            bytesReaded += bytesRead;
                        } while (bytesReaded != bufferSize);

                        bytesReaded = 0;

                        bmp = BitmapImage.Create(900, 600, 96, 96, PixelFormats.Rgb24, null, buffer, stride);
                        bmp.Freeze();

                        Action action = new Action(updateDisplay);
                        //System.Windows.Threading.Dispatcher.BeginInvoke(action);

                    } while (bytesRead != 0);
                }
            });

            thread.Start();
        }

        private void updateDisplay()
        {
           // test.Source = bmp;
        }
    }
}
