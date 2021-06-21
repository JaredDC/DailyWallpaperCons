using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Console.OutputEncoding = Encoding.UTF8;
            DailyWallpaper();
        }
        static void MainOUTPUT(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
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
            }
            catch (IOException e)
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
        static void MainSTDERR(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                using (var writer = new StreamWriter("log.txt"))
                {
                    Console.SetOut(writer);
                    //Console.Error.WriteLine("Error information written to begin");
                    DailyWallpaper();
                    //Console.Error.WriteLine("Error information written to TEST");
                    Console.Out.Close();
                    writer.Close();
                    // Console.SetOut(Console.OpenStandardOutput());
                }
            }
            catch (IOException e)
            {
                TextWriter errorWriter = Console.Error;
                errorWriter.WriteLine(e.Message);
                return;
            }

            // redirect stdout to default.
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);

            // print the log file.
            Console.WriteLine(File.ReadAllText("log.txt"));

        }

        /*TODO*/
        // generate single file excutable
        private static bool DailyWallpaper()
        {
            // Wallpaper.SetWallPaper("C:\\Users\\jared\\Pictures\\pic\\61e030ca5106db3731eb1058432c6bbb7b4b5d3efbfe63001fbd56d9ae288daf.jpg");
            Wallpaper.AddWaterMark("C:\\Users\\jared\\Pictures\\photo_of_the_day\\黑沙滩上Reynisdrangar的玄武岩_冰岛_Cavan_Images_Getty_Images_watermark.jpg",
                "D:\\jared\\coding\\DailyWallpaper\\watermark.jpg",
                "黑沙滩上Reynisdrangar的玄武岩,冰岛@Cavan Images Getty Images");

            // Creating a text file named "out" in D Drive
            var MyIni = new ConfigIni();
            var iniDict = MyIni.GetCfgFromIni();
            if (iniDict["useLocal"].ToLower().Equals("yes")) {
                var localImage = new LocalImage(iniDict["imgDir"], MyIni);
                localImage.RandomSelectOneImgToWallpaper();
            }
           
            return true;
        }

    }
}
