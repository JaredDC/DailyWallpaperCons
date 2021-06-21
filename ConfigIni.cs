using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace wallpaper_today
{
    class ConfigIni
    {
        private string iniPath;
        private string exeName = Assembly.GetExecutingAssembly().GetName().Name;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        public ConfigIni(string IniPath = null)
        {
            iniPath = new FileInfo(IniPath ?? exeName+".config.ini").FullName;
            if (!File.Exists(iniPath))
            {
                CreateDefIni();
            }
        }

        public string Read(string Key, string Section = null)
        {
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section ?? exeName, Key, "", RetVal, 255, iniPath);
            return RetVal.ToString();
        }

        public void Write(string Key, string Value, string Section = null)
        {
            WritePrivateProfileString(Section ?? exeName, Key, Value, iniPath);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? exeName);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? exeName);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public void CreateDefIni()
        {
            Write("useLocal", "yes", exeName);
            Write("useOnline", "no", exeName);
            Write("createUsageStat", "once", exeName);
            Write("want2AutoRun", "no", exeName);

            Write("ngChina", "no", "Online");
            Write("bingChina", "yes", "Online");
            Write("dailyDpotlight", "yes", "Online");
            Write("alwaysdlBingWallpaper", "yes", "Online");


            Write("imgDir", @"D:\jared\erotic\[Wanimal-Wallpaper]", "Local");
            Write("scan", "yes", "Local");

            Write("copyFolder", "None", "Local");
            Write("want2Copy", "no", "Local");

            Write("mTime", "NULL", "Local");
            Write("lastImgDir", "NULL", "Local");
            Write("wallpaper", "NULL", "Local");
        }
    }
}
