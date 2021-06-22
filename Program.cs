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
        static void MainSTD(string[] args)
        {
            DailyWallpaper();
        }
        static void Main(string[] args)
        {

            string logFile = "log.txt";
            Console.WriteLine($"Set stdoutput and stderr to file: {logFile}");
            Console.WriteLine("Please be PATIENT, the result will not be lost.");           
            Console.WriteLine("-----------");           
            using (var writer = new StreamWriter(logFile))
            {
                Console.SetOut(writer);
                Console.SetError(writer);
                //Console.Error.WriteLine("Error information written to begin");
                try
                {
                    DailyWallpaper();
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
            Console.WriteLine(File.ReadAllText("log.txt"));
        }

        /*TODO*/
        // generate single file excutable
        private static bool DailyWallpaper()
        {

            var iniFile = new ConfigIni();
            
            RunAtStartup(iniFile);
            CreateUsageText(iniFile, "USAGE.TXT");
            
            var iniDict = iniFile.GetCfgFromIni();
            if (iniDict["useLocal"].ToLower().Equals("yes")) {
                var localImage = new LocalImage(iniFile, iniDict["imgDir"]);
                localImage.RandomSelectOneImgToWallpaper();
            } else if (iniDict["useOnline"].ToLower().Equals("yes"))
            {
                //var localImage = new OnlineImage(iniDict["imgDir"], MyIni);
                //localImage.RandomSelectOneImgToWallpaper();
                var onlineImage = new OnlineImage(iniFile);
                onlineImage.RandomChoiceFromList();
                // Wallpaper.DownLoadFromUrl(@"d:\jared", "happy.jpeg", "https://www.baidu.com/img/flexible/logo/pc/result.png");
            }
            
            
            
              
            return true;
        }

        public static void RunAtStartup(ConfigIni ini)
        {
            // ConfigIni iniFile
            string want2AutoRun = ini.Read("want2AutoRun").ToLower();
            if (want2AutoRun.Equals("yes") || want2AutoRun.Equals("once"))
            {
                AutoStartupHelper.CreateAutorunShortcut();
                if (want2AutoRun.Equals("once"))
                {
                    ini.Write("want2AutoRun", "yes/no");
                }
            }
            else if (want2AutoRun.Equals("no"))
            {
                if (!AutoStartupHelper.IsAutorun())
                {
                    return;
                }
                Console.WriteLine("You don't want this program run at Windows startup.");
                AutoStartupHelper.RemoveAutorunShortcut();
            }
        }

        private static void CreateUsageText(ConfigIni ini, string textFile, string usageText=null)
        {
            if (ini.GetCfgFromIni()["createUsageStat"].ToLower().Equals("no"))
            {
                return;
            }
            if (ini.GetCfgFromIni()["createUsageStat"].ToLower().Equals("once"))
            {
                ini.UpdateIniItem("createUsageStat", "no", ini.exeName);
            }
            if (File.Exists(textFile)) {
                Console.WriteLine($"File already exists: {textFile}");
                return;
            }
            usageText = usageText ?? GetUsageText();
            File.WriteAllText(textFile, usageText);
            Console.WriteLine($"Created usage file: {textFile}");
        }

        private static string GetUsageText() {
            string usageText = "Usage for wallpaper_setter.exe/wallpaper_setter.py\r\n" +
                "AUTHOR: HDC <jared.dcx@gmail.com>\r\n" +
                "-----------------------------------------\r\n" +
                "Notice: there is only ONE file you need to configure: config.ini, \r\n" +
                "        it should be with wallpaper_setter.exe/wallpaper_setter.py\r\n" +
                "-----------------------------------------\r\n" +
                "here is a sample of config.ini:\r\n" +
                "\r\n" +
                "[OnlineOrLocal]\r\n" +
                "use_wallpapersetter = no\r\n" +
                "use_photooftheday = yes\r\n" +
                "create_usage_stat = twice\r\n" +
                "\r\n" +
                "[PhotoOfTheDay]\r\n" +
                "ngchina = no\r\n" +
                "bingchina = yes\r\n" +
                "daily.spotlight = yes\r\n" +
                "alwaysdownload.bing.wallpaper = yes\r\n" +
                "\r\n" +
                "[WallpaperSetter]\r\n" +
                "img_dir = C:\\Users\\SOMEONE\\Pictures\r\n" +
                "copy_folder = None\r\n" +
                "want2copy = no\r\n" +
                "scan = yes\r\n" +
                "mtime = None\r\n" +
                "last_img_dir = None\r\n" +
                "wallpaper = C:\\Users\\SOMEONE\\Pictures\\OnePicture.jpeg\r\n" +
                "\r\n" +
                "---------------------\r\n" +
                "Section OnlineOrLocal\r\n" +
                "1. use_wallpapersetter            Download the image and set it as wallpaper.\r\n" +
                "2. use_photooftheday              Use local image, which means use \"Section WallpaperSetter\" feature.\r\n" +
                "3. create_usage_stat              Create and usage file flag: always, once, no\r\n" +
                "                                    always: when 'USAGE.TXT' doesn't exist, always create\r\n" +
                "                                    twice:  you can't delete two times\r\n" +
                "                                    once:   when 'USAGE.TXT' doesn't exist, create once, you can delete, it won't create next time.\r\n" +
                "                                    no:     literally.\r\n" +
                "--------\r\n" +
                "Section PhotoOfTheDay\r\n" +
                "1. ngchina                        Download \"ngchina\" 's image and set it as wallpaper\r\n" +
                "2. bingchina                      Download \"bingchina\" 's image and set it as wallpaper\r\n" +
                "3. daily.spotlight                Copy the image from daily.spotlight folder and set it as wallpaper \r\n" +
                "                                    [You have to open the feature in Windows10]\r\n" +
                "4. alwaysdownload.bing.wallpaper  Always download bingchina wallpaper\r\n" +
                "--------\r\n" +
                "Section WallpaperSetter\r\n" +
                "1. img_dir:                       The program will scan this folder and select a image as wallpaper\r\n" +
                "2. copy_folder:                   Copy all suitable pictures to this folder from copy_folder, control by 'want2copy'\r\n" +
                "3. want2copy:                     Controlling the action of COPYING, it has two options: yes, no\r\n" +
                "4. scan:                          Controlling the action of SCANNING, it has three options: yes, no, force\r\n" +
                "                                    yes:   when 'img_dir' has been modified by OS, scan and update '_img_list.txt'\r\n" +
                "                                    no:    never scan 'img_dir' unless '_img_list.txt' doesn't exist.\r\n" +
                "                                    force: Mandatory scan 'img_dir' and update '_img_list.txt'\r\n" +
                "5. mtime:                         The modified time of 'img_dir'\r\n" +
                "6. last_img_dir:                  Literally.\r\n" +
                "7. wallpaper:                     Wallpaper setting history.\r\n" +
                "-----------------------------------------\r\n" +
                "FOR FREEDOM!";
            return usageText;
        }
    }
}
