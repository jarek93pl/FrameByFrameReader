using Emgu.CV;
using Emgu.CV.Dai;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.IO;
public class FrameReader
{
    static string textName = "frame_";
    public static void ReadFrames(string videoPath, int perFrame = 1)
    {
        try
        {
            using (var capture = new VideoCapture(videoPath))
            {
                if (capture.IsOpened)
                {
                    Mat frame = new Mat();
                    int frameCount = 0;

                    while (capture.Read(frame))
                    {
                        // Process the frame (e.g., display it, save it)
                        if (!frame.IsEmpty)
                        {
                            // Convert to Bitmap for easier display or manipulation
                            using (var bitmap = Emgu.CV.BitmapExtension.ToBitmap(frame))
                            {
                                // Example: Save the frame as a JPEG
                                if (frameCount % perFrame == 0)
                                {
                                    bitmap.Save($"{textName}{frameCount}.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                            }
                        }
                        frameCount++;
                    }
                    Console.WriteLine($"Read {frameCount} frames.");
                }
                else
                {
                    Console.WriteLine("Could not open video file.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
    public static void Main(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            textName = "FrameLoadFromFile_";
            args = File.ReadAllLines("args.txt");
        }

        if (args == null || args.Length == 0)
        {
            Console.WriteLine("give a path and, per frame you want get");
        }
        else
        {

            int perFrame = args.Length > 1 ? Convert.ToInt32(args[1]) : 0;
            ReadFrames(args[0], perFrame);
        }
    }
}