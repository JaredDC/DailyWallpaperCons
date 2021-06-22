using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class OnlineImage
    {
        private ConfigIni ini;
        private string path;
        private string wallpaperWMK;

        public OnlineImage(ConfigIni ini, string path = null)
        {
            this.ini = ini;
            this.path = path;
            if (String.IsNullOrEmpty(path))
            {
                if (this.ini.Read("saveDir", "Online").ToLower().Equals("null")) 
                { 
                    var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    myPictures = Path.Combine(myPictures, ini.exeName);
                    Console.WriteLine($"-> The downloaded picture will be saved to: {myPictures}");
                    ini.UpdateIniItem("saveDir", myPictures, "Online");
                    this.path = myPictures;
                } else
                {
                    this.path = this.ini.Read("saveDir", "Online");
                }

            }
            if (!Directory.Exists(this.path))
            {
                Directory.CreateDirectory(this.path);
            }
            

        }

        public async Task RandomChoiceFromList()
		{
            // online
            var iniDict = new Dictionary<string, string>();
            iniDict.Add("bingChina", this.ini.Read("bingChina", "Online"));
            iniDict.Add("dailySpotlight", this.ini.Read("dailySpotlight", "Online"));
            bool alwaysDLBingWallpaper = false;
            if (this.ini.Read("alwaysDLBingWallpaper", "Online").ToLower().Equals("yes"))
            {
                alwaysDLBingWallpaper = true;
            }

            var onlineList = new List<string>();
            foreach (string key in iniDict.Keys)
            {
                if (this.ini.Read(key, "Online").ToLower().Equals("yes"))
                {
                    onlineList.Add(key);
                }
            }
            var random = new Random();
			int index = random.Next(onlineList.Count);
            if (alwaysDLBingWallpaper)
            {
                await BingChina();
            }

            string choice = onlineList[index];
            Console.WriteLine($"-> The choice is: {choice}");
            switch (choice)
            {
                case "bingChina":
                    var bingList = await BingChina();
                    var copyRight = bingList[1];
                    Console.WriteLine(copyRight);
                    if (!File.Exists(wallpaperWMK))
                    {
                        var oriImg = bingList[0];
                        Wallpaper.AddWaterMark(oriImg, wallpaperWMK, copyRight);
                    }
                    Wallpaper.SetWallPaper(wallpaperWMK);
                    ini.UpdateIniItem("wallpaper", wallpaperWMK + "    " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LOG");
                    break;
           
                case "dailySpotlight":
                    var wallpaper = DailySpotlight();
                    Wallpaper.SetWallPaper(wallpaper);
                    ini.UpdateIniItem("wallpaper", wallpaper + "    " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LOG");
                    break;

                default:
                    Console.WriteLine("Default.");
                    break;
            }
        }
        /*
         * Input: url
         * Return: img-url, img_name
         */

        public void WebCrawler(string url)
        {

        }

        public async Task<List<string>> BingChina()
        {
            var bingImg = await new BingImageProvider().GetImage(check:true);
            // remove illegal characters
            // var file_name = string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
            // replace illegal characters with _
            var file_name = string.Join("_", bingImg.Copyright.Split(Path.GetInvalidFileNameChars()));
            string wallpaper = Path.Combine(path, file_name + ".jpg");
            wallpaperWMK = Path.Combine(path, file_name + "-WMK.jpg");
            if (!File.Exists(wallpaperWMK)) {
                // Don't download the picture again and again.
                var img = await new BingImageProvider().GetImage(check:false);
                img.Img.Save(wallpaper, System.Drawing.Imaging.ImageFormat.Jpeg);
            }           
            return new List<string> { wallpaper, bingImg.Copyright, bingImg.CopyrightLink };
        }

        public static void PrintAllSystemEnvironmentInfo()
    {
            foreach (System.Collections.DictionaryEntry e in System.Environment.GetEnvironmentVariables())
            {
                Console.WriteLine(e.Key + ":" + e.Value);
            }
        }
        private string GetDailySpotlightDir()
        {
            if (!ini.GetCfgFromIni()["dailySpotlightDir"].ToLower().Equals("auto"))
            {
                var dir = ini.GetCfgFromIni()["dailySpotlightDir"];
                if (!Directory.Exists(dir)) {
                    throw new DirectoryNotFoundException($"dailySpotlightDir invalid: {dir}");
                }
                return dir;
            }
            var exception = new Exception("ERROR: it should be ONE dailySpotlightDir, tell me what to do next.\r\n" +
                    "The sample path is: " +
                    @"C:\Users\jared\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets" + Environment.NewLine+
                    "\r\nFIRST OF ALL: \r\n    Turn on dailySpotlight feature in Windows 10 by \r\n" +
                    "  Google/Baidu \"How to enable Windows Spotlight?\" \r\n\r\n" +
                    "You can specify the PATH in the config.ini by:\r\n" +
                    @"  dailySpotlightDir=C:\your_path" + "\r\n\r\n" + 
                    "Or turn off dailySpotlight feature by:\r\n" +
                    "  dailySpotlight=no\r\n"
                    ); 
            string LOCALAPPDATA = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var pkg = Path.Combine(LOCALAPPDATA, "Packages");
            var contentDeliveryManager = Directory.GetDirectories(pkg, "*ContentDeliveryManager*", SearchOption.AllDirectories);
            if (!contentDeliveryManager.Length.Equals(1))
            {
                throw exception;
            } 
            var assets = Directory.GetDirectories(contentDeliveryManager[0], "Assets", SearchOption.AllDirectories);
            if (!assets.Length.Equals(1))
            {
                throw exception;
            }
            var dailySpotlightDir = assets[0];
            return dailySpotlightDir;
        }
        public string DailySpotlight()
        {
            string dailySpotlightDir = GetDailySpotlightDir();
            var jpegFiles = new List<FileInfo>();
            var wallpaperDict = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(dailySpotlightDir, "*", SearchOption.AllDirectories))
            {
                /*try { 
                    System.Drawing.Image imgInput = System.Drawing.Image.FromFile(file);
                    System.Drawing.Graphics gInput = System.Drawing.Graphics.FromImage(imgInput);
                    System.Drawing.Imaging.ImageFormat thisFormat = imgInput.RawFormat;
                    Console.WriteLine("It is image");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("It is not image");
                }*/

                long length = new FileInfo(file).Length / 1024;
                if (length > 100)
                {
                   /* Bitmap img = new Bitmap(file);
                    Console.WriteLine(img.RawFormat.ToString());
                    img.Dispose();*/

                    Image img = Image.FromFile(file);
                    if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(img.RawFormat))
                    {
                        if (img.Width > 1900 && (img.Width + 0.0 / img.Height > 1.4))
                        {
                            var jpegfi = new FileInfo(file);
                            jpegFiles.Add(jpegfi);
                            var dest = Path.Combine(path, jpegfi.CreationTime.ToString("yyyy-MMdd_HH-mm-ss") + ".jpeg");
                            wallpaperDict.Add(jpegfi.Name, dest);
                            if (!File.Exists(dest))
                            {
                                jpegfi.CopyTo(dest);
                                Console.WriteLine($"copy to: {dest}");
                            }
                            // 
                        }
                        else
                        {        
                            var jpegPhone = new FileInfo(file);
                            var dest = Path.Combine(path, jpegPhone.CreationTime.ToString("yyyy-MMdd_HH-mm-ss") + "-Phone.jpeg");
                            if (!File.Exists(dest))
                            {
                                jpegPhone.CopyTo(dest);
                                Console.WriteLine($"copy to: {dest}");
                            }
                        }
                    } else if (System.Drawing.Imaging.ImageFormat.Png.Equals(img.RawFormat))
                    {
                        Console.WriteLine("PNG: abandon");
                        
                    }                   
                    img.Dispose();
                }             
            }
            List <FileInfo> jpegFilesOrdered     = jpegFiles.OrderByDescending(x => x.CreationTime).ToList();
            return wallpaperDict[jpegFilesOrdered[0].Name];
        }      
    }
}
