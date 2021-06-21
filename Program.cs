using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wallpaper_today
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (var writer = new StreamWriter("log.txt"))
                {
                    Console.SetOut(writer);
                    Console.SetError(writer);

                    //Console.Error.WriteLine("Error information written to begin");
                    DailyWallpaper();
                    //Console.Error.WriteLine("Error information written to TEST");
                    Console.Out.Close();
                    Console.Error.Close();
                    writer.Close();
                    // Console.SetOut(Console.OpenStandardOutput());
                }
            } catch (IOException e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
                return;
            }
            // redirect stderr to default.
            var standardError = new StreamWriter(Console.OpenStandardError());
            standardError.AutoFlush = true;
            Console.SetError(standardError);

            // redirect stdout to default.
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
            
            // print the log file.
            Console.WriteLine(File.ReadAllText("log.txt"));

        }

        private static bool DailyWallpaper()
        {
            // Wallpaper.SetWallPaper("C:\\Users\\jared\\Pictures\\pic\\61e030ca5106db3731eb1058432c6bbb7b4b5d3efbfe63001fbd56d9ae288daf.jpg");
            Wallpaper.AddWaterMark("C:\\Users\\jared\\Pictures\\photo_of_the_day\\黑沙滩上Reynisdrangar的玄武岩_冰岛_Cavan_Images_Getty_Images_watermark.jpg",
                "D:\\jared\\coding\\wallpaper_today\\watermark.jpg",
                "黑沙滩上Reynisdrangar的玄武岩,冰岛@Cavan Images Getty Images");
            //new LocalImage(@"C:\Users\jared\Pictures\新建文件夹").ScanLocalPath();
            new LocalImage(@"C:\Users\jared\Pictures\新建文件夹").ShoulditUpdate();
            //new LocalImage("C:\\Users\\jared\\Pictures\\photo_of_the_day").ScanLocalPath();
            //new LocalImage("D:\\jared\\Videos\\movies\\seen").ScanLocalPath();
            // Creating a text file named "out" in D Drive

            return true;
        }

    }
}
