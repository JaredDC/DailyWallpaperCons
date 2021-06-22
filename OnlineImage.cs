using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class OnlineImage
    {
        private ConfigIni ini;
        private string path;
        private Dictionary<string, string> iniDict;
        private bool alwaysdlBingWallpaper = false;


        public OnlineImage(ConfigIni ini, string path=null) {
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
            // online
            iniDict = new Dictionary<string, string>();
            
            iniDict.Add("ngChina", this.ini.Read("ngChina", "Online"));
            iniDict.Add("bingChina", this.ini.Read("bingChina", "Online"));
            iniDict.Add("dailyDpotlight", this.ini.Read("dailyDpotlight", "Online"));
            if (this.ini.Read("alwaysdlBingWallpaper", "Online").ToLower().Equals("yes"))
            {
                alwaysdlBingWallpaper = true;
            }
            // iniDict.Add("alwaysdlBingWallpaper", );
            var onlineList = new List<string>();
            foreach (string key in iniDict.Keys)
            {
                if (this.ini.Read(key, "Online").ToLower().Equals("yes"))
                {
                    onlineList.Add(key);
                    Console.WriteLine(key);
                }
            }

        }
        /*
         * Input: url
         * Return: img-url, img_name
         */
        public void WebCrawler(string url)
        {

        }

        public void BingChina()
        {

        }

        public void NgChina()
        {

        }

        public void DailyDpotlight()
        {

        }
    }
}
