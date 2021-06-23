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
        static async Task MainSTD(string[] args)
        {
            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            await DailyWallpaper(exeName);
        }
        static async Task Main(string[] args)
        {

            var exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            string logFile = new FileInfo(exeName + ".log.txt").FullName;
            Console.WriteLine($"Set stdoutput and stderr to file: {logFile}");
            Console.WriteLine("Please be PATIENT, the result will not be lost.");           
            Console.WriteLine($"------  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  ------");
            using (var writer = new StreamWriter(logFile))
            {
                Console.SetOut(writer);
                Console.SetError(writer);
                //Console.Error.WriteLine("Error information written to begin");
                try
                {
                    await DailyWallpaper(exeName);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                }
                //Console.Error.WriteLine("Error information written to TEST");
                Console.Out.Close();
                Console.Error.Close();
                writer.Close();
                // Console.SetOut(Console.OpenStandardOutput());
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
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(File.ReadAllText(logFile));
        }

        /*TODO*/
        // generate single file excutable
        private async static Task DailyWallpaper(string exeName)
        {
            var iniFile = new ConfigIni(exeName: exeName);
            iniFile.RunAtStartup();
            iniFile.CreateUsageText(exeName + ".USAGE.TXT");
            var iniDict = iniFile.GetCfgFromIni();
            if (iniDict["useLocal"].ToLower().Equals("yes")) {
                var localImage = new LocalImage(iniFile, iniDict["imgDir"]);
                localImage.RandomSelectOneImgToWallpaper();
            } else if (iniDict["useOnline"].ToLower().Equals("yes"))
            {
                var onlineImage = new OnlineImage(iniFile);
                await onlineImage.RandomChoiceFromList();
            }             
        }
    }
}
